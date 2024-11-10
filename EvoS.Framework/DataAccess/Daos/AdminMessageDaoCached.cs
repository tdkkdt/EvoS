using System.Collections.Generic;
using System.Linq;
using BitFaster.Caching.Lru;

namespace EvoS.Framework.DataAccess.Daos
{
    public class AdminMessageDaoCached: AdminMessageDao
    {
        private readonly AdminMessageDao dao;
        
        private const int Capacity = 128;
        private readonly FastConcurrentLru<long, List<AdminMessageDao.AdminMessage>> cache = new(Capacity);

        public AdminMessageDaoCached(AdminMessageDao dao)
        {
            this.dao = dao;
        }

        private void Cache(AdminMessageDao.AdminMessage msg)
        {
            List<AdminMessageDao.AdminMessage> messages;
            lock (cache)
            {
                messages = cache
                    .GetOrAdd(msg.accountId, _ => new List<AdminMessageDao.AdminMessage>());
            }
            lock (messages)
            {
                messages.RemoveAll(m => m.createdAt == msg.createdAt && m.adminAccountId == msg.adminAccountId);
                messages.Add(msg);
            }
        }

        public AdminMessageDao.AdminMessage FindPending(long accountId)
        {
            if (cache.TryGet(accountId, out var cachedMessages))
            {
                return cachedMessages
                    .Where(m => !m.viewed)
                    .MinBy(m => m.createdAt);
            }

            AdminMessageDao.AdminMessage msg = dao.FindPending(accountId);
            if (msg is not null)
            {
                Cache(msg);
            }
            return msg;
        }

        public List<AdminMessageDao.AdminMessage> Find(long accountId)
        {
            if (cache.TryGet(accountId, out var cachedMessages))
            {
                return cachedMessages
                    .OrderBy(m => m.viewed)
                    .ThenByDescending(m => m.createdAt)
                    .Take(AdminMessageDao.LIMIT)
                    .ToList();
            }

            List<AdminMessageDao.AdminMessage> dbMessages = dao.Find(accountId);
            dbMessages.ForEach(Cache);
            return dbMessages;
        }
        
        public void Save(AdminMessageDao.AdminMessage msg)
        {
            dao.Save(msg);
            Cache(msg);
        }
    }
}