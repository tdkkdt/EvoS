using System;
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
            data.UpdateDate = DateTime.UtcNow;
            dao.UpdateAccount(data);
            Cache(data);
        }

        public void UpdateAccountComponent(PersistedAccountData data)
        {
            data.UpdateDate = DateTime.UtcNow;
            dao.UpdateAccountComponent(data);
            Cache(data);
        }

        public void UpdateAdminComponent(PersistedAccountData data)
        {
            data.UpdateDate = DateTime.UtcNow;
            dao.UpdateAdminComponent(data);
            Cache(data);
        }

        public void UpdateBankComponent(PersistedAccountData data)
        {
            data.UpdateDate = DateTime.UtcNow;
            dao.UpdateBankComponent(data);
            Cache(data);
        }

        public void UpdateSocialComponent(PersistedAccountData data)
        {
            data.UpdateDate = DateTime.UtcNow;
            dao.UpdateSocialComponent(data);
            Cache(data);
        }

        public void UpdateLastCharacter(PersistedAccountData data)
        {
            data.UpdateDate = DateTime.UtcNow;
            dao.UpdateLastCharacter(data);
            Cache(data);
        }

        public void UpdateCharacterComponent(PersistedAccountData data, CharacterType characterType)
        {
            data.UpdateDate = DateTime.UtcNow;
            dao.UpdateCharacterComponent(data, characterType);
            Cache(data);
        }
    }
}