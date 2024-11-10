using System;
using System.Collections.Generic;
using System.Linq;
using BitFaster.Caching.Lru;
using EvoS.Framework.DataAccess.Mock;

namespace EvoS.Framework.DataAccess.Daos
{
    public class RegistrationCodeDaoCached: RegistrationCodeDao
    {
        private readonly RegistrationCodeDao dao;
    
        private const int Capacity = 128;
        private readonly FastConcurrentLru<string, RegistrationCodeDao.RegistrationCodeEntry> cache = new(Capacity);

        public RegistrationCodeDaoCached(RegistrationCodeDao dao)
        {
            this.dao = dao;
        }

        private void Cache(RegistrationCodeDao.RegistrationCodeEntry entry)
        {
            cache.AddOrUpdate(entry.Code, entry);
        }

        public RegistrationCodeDao.RegistrationCodeEntry Find(string code)
        {
            if (cache.TryGet(code, out var entry))
            {
                return entry;
            }

            var nonCachedEntry = dao.Find(code);
            if (nonCachedEntry != null)
            {
                Cache(nonCachedEntry);
            }
            return nonCachedEntry;
        }

        public List<RegistrationCodeDao.RegistrationCodeEntry> FindBefore(int limit, DateTime dateTime)
        {
            if (dao is RegistrationCodeMockDao)
            {
                return cache
                    .Select(x => x.Value)
                    .Where(x => x.IssuedAt < dateTime)
                    .OrderByDescending(x => x.IssuedAt)
                    .Take(limit)
                    .ToList();
            }
            
            List<RegistrationCodeDao.RegistrationCodeEntry> daoEntries = dao.FindBefore(limit, dateTime);
            daoEntries.ForEach(Cache);
            return daoEntries;
        }

        public List<RegistrationCodeDao.RegistrationCodeEntry> FindAll(int limit, int offset)
        {
            if (dao is RegistrationCodeMockDao)
            {
                return cache
                    .Select(x => x.Value)
                    .OrderByDescending(x => x.IssuedAt)
                    .Skip(offset)
                    .Take(limit)
                    .ToList();
            }
            
            List<RegistrationCodeDao.RegistrationCodeEntry> daoEntries = dao.FindAll(limit, offset);
            daoEntries.ForEach(Cache);
            return daoEntries;
        }

        public void Save(RegistrationCodeDao.RegistrationCodeEntry entry)
        {
            dao.Save(entry);
            Cache(entry);
        }
    }
}