using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EvoS.Framework.DataAccess.Mock;

namespace EvoS.Framework.DataAccess.Daos
{
    public class RegistrationCodeDaoCached: RegistrationCodeDao
    {
        private readonly RegistrationCodeDao dao;
        private readonly ConcurrentDictionary<string, RegistrationCodeDao.RegistrationCodeEntry> cache = 
            new ConcurrentDictionary<string, RegistrationCodeDao.RegistrationCodeEntry>();

        public RegistrationCodeDaoCached(RegistrationCodeDao dao)
        {
            this.dao = dao;
        }

        private void Cache(RegistrationCodeDao.RegistrationCodeEntry entry)
        {
            cache.AddOrUpdate(entry.Code, entry, (k, v) => entry);
        }

        public RegistrationCodeDao.RegistrationCodeEntry Find(string code)
        {
            if (cache.TryGetValue(code, out var entry))
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

        public List<RegistrationCodeDao.RegistrationCodeEntry> FindBefore(DateTime dateTime)
        {
            if (dao is RegistrationCodeMockDao)
            {
                return cache.Values
                    .Where(x => x.IssuedAt < dateTime)
                    .OrderByDescending(x => x.IssuedAt)
                    .Take(RegistrationCodeDao.LIMIT)
                    .ToList();
            }
            
            List<RegistrationCodeDao.RegistrationCodeEntry> daoEntries = dao.FindBefore(dateTime);
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