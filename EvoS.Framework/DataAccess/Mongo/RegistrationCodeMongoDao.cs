using System;
using System.Collections.Generic;
using EvoS.Framework.DataAccess.Daos;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class RegistrationCodeMongoDao : MongoDao<string, RegistrationCodeDao.RegistrationCodeEntry>, RegistrationCodeDao
    {
        public RegistrationCodeMongoDao() : base("registration_codes")
        {
        }

        public RegistrationCodeDao.RegistrationCodeEntry Find(string code)
        {
            return c.Find(f.Eq("Code", code)).FirstOrDefault();
        }

        public List<RegistrationCodeDao.RegistrationCodeEntry> FindBefore(DateTime dateTime)
        {
            return c
                .Find(f.Lt("IssuedAt", dateTime))
                .Sort(s.Descending("IssuedAt"))
                .Limit(RegistrationCodeDao.LIMIT)
                .ToList();
        }

        public void Save(RegistrationCodeDao.RegistrationCodeEntry entry)
        {
            insert(entry.Code, entry);
        }
    }
} 