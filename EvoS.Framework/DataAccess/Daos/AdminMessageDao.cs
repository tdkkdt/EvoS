using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EvoS.Framework.DataAccess.Daos
{
    public interface AdminMessageDao
    {
        protected const int LIMIT = 15;

        public AdminMessage FindPending(long accountId);
        public List<AdminMessage> Find(long accountId);
        public void Save(AdminMessage msg);


        public class AdminMessage
        {
            [BsonId] public ObjectId _id;
            public required long accountId;
            public bool viewed;
            public required DateTime createdAt;
            public required long adminAccountId;
            public required string message;
            public DateTime viewedAt;
        }
    }
}