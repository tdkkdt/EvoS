using System.Collections.Generic;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Misc;
using EvoS.Framework.Network.Static;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class MatchHistoryMongoDao : MongoDao<long, MatchHistoryDao.MatchEntry>, MatchHistoryDao
    {
        public MatchHistoryMongoDao() : base(
            "match_history", 
            new CreateIndexModel<MatchHistoryDao.MatchEntry>(Builders<MatchHistoryDao.MatchEntry>.IndexKeys
                .Ascending(entry => entry.AccountId)
                .Descending(entry => entry.Data.CreateDate)))
        {
        }

        public List<PersistedCharacterMatchData> Find(long accountId)
        {
            return c.Find(
                f.Eq("AccountId", accountId))
                .SortByDescending(m => m.Data.CreateDate)
                .Limit(MatchHistoryDao.LIMIT)
                .Project(m => m.Data)
                .ToList();
        }

        public void Save(ICollection<MatchHistoryDao.MatchEntry> matchEntries)
        {
            if (!matchEntries.IsNullOrEmpty())
            {
                c.InsertMany(matchEntries);
            }
        }
    }
} 