using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;
using log4net;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class AccountMongoDao : MongoDao<long, PersistedAccountData>, AccountDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AccountMongoDao));
        
        public AccountMongoDao() : base("accounts")
        {
        }

        public PersistedAccountData GetAccount(long accountId)
        {
            return findById(accountId);
        }

        public void CreateAccount(PersistedAccountData data)
        {
            log.Info($"New player {data.AccountId}: {data}");
            UpdateAccount(data);
        }

        public void UpdateAccount(PersistedAccountData data)
        {
            insert(data.AccountId, data);
        }
    }
} 