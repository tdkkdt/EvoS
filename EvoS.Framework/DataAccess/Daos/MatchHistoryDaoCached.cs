using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.DataAccess.Daos
{
    public class MatchHistoryDaoCached: MatchHistoryDao
    {
        private readonly MatchHistoryDao dao;
        private readonly ConcurrentDictionary<long, ConcurrentQueue<PersistedCharacterMatchData>> cache =
            new ConcurrentDictionary<long, ConcurrentQueue<PersistedCharacterMatchData>>();

        public MatchHistoryDaoCached(MatchHistoryDao dao)
        {
            this.dao = dao;
        }

        private void Cache(ICollection<MatchHistoryDao.MatchEntry> matchEntries)
        {
            var byAccount = matchEntries.GroupBy(m => m.AccountId);

            foreach (IGrouping<long, MatchHistoryDao.MatchEntry> accountToMatches in byAccount)
            {
                ConcurrentQueue<PersistedCharacterMatchData> matches = cache
                    .GetOrAdd(accountToMatches.Key, _ => new ConcurrentQueue<PersistedCharacterMatchData>());
                foreach (MatchHistoryDao.MatchEntry match in accountToMatches)
                {
                    matches.Enqueue(match.Data);
                }

                lock (matches)
                {
                    while (matches.Count > MatchHistoryDao.LIMIT)
                    {
                        matches.TryDequeue(out _);
                    }
                }
            }
        }

        public List<PersistedCharacterMatchData> Find(long accountId)
        {
            if (cache.TryGetValue(accountId, out var cachedMatches))
            {
                return cachedMatches.ToList();
            }

            List<PersistedCharacterMatchData> dbMatches = dao.Find(accountId);
            Cache(dbMatches
                .Select(d => new MatchHistoryDao.MatchEntry { AccountId = accountId, Data = d})
                .ToList());
            return dbMatches;
        }
        
        public void Save(ICollection<MatchHistoryDao.MatchEntry> matchEntries)
        {
            dao.Save(matchEntries);
            Cache(matchEntries);
        }
    }
}