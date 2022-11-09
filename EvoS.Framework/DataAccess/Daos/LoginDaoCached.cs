using System.Collections.Concurrent;

namespace EvoS.Framework.DataAccess.Daos
{
    public class LoginDaoCached: LoginDao
    {
        private readonly LoginDao dao;
        private readonly ConcurrentDictionary<long, LoginDao.LoginEntry> cache = new ConcurrentDictionary<long, LoginDao.LoginEntry>();
        private readonly ConcurrentDictionary<string, LoginDao.LoginEntry> usernameCache = new ConcurrentDictionary<string, LoginDao.LoginEntry>();

        public LoginDaoCached(LoginDao dao)
        {
            this.dao = dao;
        }

        private void Cache(LoginDao.LoginEntry account)
        {
            cache.AddOrUpdate(account.AccountId, account, (k, v) => account);
            usernameCache.AddOrUpdate(account.Username, account, (k, v) => account);
        }

        public LoginDao.LoginEntry Find(string username)
        {
            if (usernameCache.TryGetValue(username, out var account))
            {
                return account;
            }

            var nonCachedAccount = dao.Find(username);
            if (nonCachedAccount != null)
            {
                Cache(nonCachedAccount);
            }
            return nonCachedAccount;
        }

        public LoginDao.LoginEntry Find(long accountId)
        {
            if (cache.TryGetValue(accountId, out var account))
            {
                return account;
            }

            var nonCachedAccount = dao.Find(accountId);
            if (nonCachedAccount != null)
            {
                Cache(nonCachedAccount);
            }
            return nonCachedAccount;
        }

        public void Save(LoginDao.LoginEntry entry)
        {
            dao.Save(entry);
            Cache(entry);
        }
    }
}