using System;
using System.Collections.Generic;
using EvoS.Framework.DataAccess.Daos;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class RegistrationCodeMongoDao : MongoDao<string, RegistrationCodeDao.RegistrationCodeEntry>, RegistrationCodeDao
    {
        public RegistrationCodeMongoDao() : base(
            "registration_codes", 
            new CreateIndexModel<RegistrationCodeDao.RegistrationCodeEntry>(Builders<RegistrationCodeDao.RegistrationCodeEntry>.IndexKeys
                .Descending(entry => entry.IssuedAt)))
        {
        }

        public RegistrationCodeDao.RegistrationCodeEntry Find(string code)
        {
            return c.Find(f.Eq("Code", code)).FirstOrDefault();
        }

        public List<RegistrationCodeDao.RegistrationCodeEntry> FindBefore(int limit, DateTime dateTime)
        {
            return c
                .Find(f.Lt("IssuedAt", dateTime))
                .Sort(s.Descending("IssuedAt"))
                .Limit(limit)
                .ToList();
        }

        public List<RegistrationCodeDao.RegistrationCodeEntry> FindAll(int limit, int offset)
        {
            return c
                .Find(f.Empty)
                .Sort(s.Descending("IssuedAt"))
                .Skip(offset)
                .Limit(limit)
                .ToList();
        }

        public void Save(RegistrationCodeDao.RegistrationCodeEntry entry)
        {
            insert(entry.Code, entry);
        }
    }
} 