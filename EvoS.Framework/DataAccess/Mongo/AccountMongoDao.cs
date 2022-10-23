using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Logging;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class AccountMongoDao : MongoDao<long, PersistedAccountData>, AccountDao
    {
        public AccountMongoDao() : base("accounts")
        {
        }

        public PersistedAccountData GetAccount(long accountId)
        {
            return findById(accountId);
        }

        public void CreateAccount(PersistedAccountData data)
        {
            Log.Print(LogType.Lobby, $"New player {data.AccountId}: {data}");
            UpdateAccount(data);
        }

        public void UpdateAccount(PersistedAccountData data)
        {
            insert(data.AccountId, data);
        }
    }
} 