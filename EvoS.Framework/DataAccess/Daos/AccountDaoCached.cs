using System.Collections.Concurrent;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.DataAccess.Daos
{
    public class AccountDaoCached: AccountDao
    {
        private readonly AccountDao dao;
        private readonly ConcurrentDictionary<long, PersistedAccountData> cache = new ConcurrentDictionary<long, PersistedAccountData>();

        public AccountDaoCached(AccountDao dao)
        {
            this.dao = dao;
        }

        private void Cache(PersistedAccountData account)
        {
            cache.AddOrUpdate(account.AccountId, account, (k, v) => account);
        }

        public PersistedAccountData GetAccount(long accountId)
        {
            if (cache.TryGetValue(accountId, out var account))
            {
                return account;
            }

            var nonCachedAccount = dao.GetAccount(accountId);
            if (nonCachedAccount != null)
            {
                Cache(nonCachedAccount);
            }
            return nonCachedAccount;
        }

        public void CreateAccount(PersistedAccountData data)
        {
            dao.CreateAccount(data);
            Cache(data);
        }

        public void UpdateAccount(PersistedAccountData data)
        {
            dao.UpdateAccount(data);
            Cache(data);
        }
    }
}