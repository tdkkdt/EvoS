using System.Collections.Generic;
using EvoS.Framework.DataAccess.Daos;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class AdminMessageMongoDao : MongoDao<ObjectId, AdminMessageDao.AdminMessage>, AdminMessageDao
    {
        public AdminMessageMongoDao() : base("admin_messages")
        {
        }

        public AdminMessageDao.AdminMessage FindPending(long accountId)
        {
            return c.Find(
                    f.And(
                        f.Eq("accountId", accountId),
                        f.Eq("viewed", false)))
                .Sort(s.Ascending("createdAt"))
                .Limit(1)
                .FirstOrDefault();
        }

        List<AdminMessageDao.AdminMessage> AdminMessageDao.Find(long accountId)
        {
            return c.Find(f.Eq("accountId", accountId))
                .Sort(s.Ascending("viewed").Descending("createdAt"))
                .Limit(AdminMessageDao.LIMIT)
                .ToList();
        }

        public void Save(AdminMessageDao.AdminMessage msg)
        {
            c.InsertOne(msg);
        }
    }
} 