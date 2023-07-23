using System.Collections.Generic;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.DataAccess.Mock
{
    public class MatchHistoryMockDao: MatchHistoryDao
    {

        public List<PersistedCharacterMatchData> Find(long accountId)
        {
            return new List<PersistedCharacterMatchData>();
        }

        public void Save(ICollection<MatchHistoryDao.MatchEntry> matchEntries)
        {
        }
    }
}