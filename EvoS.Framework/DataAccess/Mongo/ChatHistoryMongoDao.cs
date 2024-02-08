using System;
using System.Collections.Generic;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess.Daos;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class ChatHistoryMongoDao : MongoDao<ObjectId, ChatHistoryDao.Entry>, ChatHistoryDao
    {
        public ChatHistoryMongoDao() : base(
            "chat_history", 
            new CreateIndexModel<ChatHistoryDao.Entry>(Builders<ChatHistoryDao.Entry>.IndexKeys
                .Ascending(msg => msg.sender)
                .Ascending(msg => msg.time)), 
            new CreateIndexModel<ChatHistoryDao.Entry>(Builders<ChatHistoryDao.Entry>.IndexKeys
                .Ascending(msg => msg.recipients)
                .Ascending(msg => msg.time)), 
            new CreateIndexModel<ChatHistoryDao.Entry>(Builders<ChatHistoryDao.Entry>.IndexKeys
                .Ascending(msg => msg.blockedRecipients)
                .Ascending(msg => msg.time)),
            new CreateIndexModel<ChatHistoryDao.Entry>(Builders<ChatHistoryDao.Entry>
                    .IndexKeys
                    .Ascending(msg => msg.time),
                new CreateIndexOptions<ChatHistoryDao.Entry>
                {
                    PartialFilterExpression = f.Eq(e => e.consoleMessageType, ConsoleMessageType.GlobalChat)
                }))
        {
        }

        public List<ChatHistoryDao.Entry> GetRelevantMessages(
            long accountId,
            bool includeBlocked,
            DateTime afterTime,
            DateTime beforeTime,
            int limit)
        {
            var baseCond = f.Or(
                f.Eq(e => e.sender, accountId),
                f.AnyEq(e => e.recipients, accountId),
                f.Eq(e => e.consoleMessageType, ConsoleMessageType.GlobalChat));
            var cond = includeBlocked
                ? f.Or(baseCond, f.AnyEq(e => e.blockedRecipients, accountId))
                : baseCond;
            return c
                .Find(f.And(
                    f.Lt(e => e.time, beforeTime),
                    f.Gte(e => e.time, afterTime),
                    cond))
                .Sort(s.Ascending(e => e.time))
                .Limit(limit)
                .ToList();
        }

        public void Save(ChatHistoryDao.Entry entry)
        {
            insert(entry._id, entry);
        }
    }
} 