using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EvoS.Framework.Auth;
using EvoS.Framework.DataAccess.Mock;

namespace EvoS.Framework.DataAccess.Daos
{
    public class LoginDaoCached: LoginDao
    {
        private readonly LoginDao dao;
        private readonly ConcurrentDictionary<long, LoginDao.LoginEntry> cache = new ConcurrentDictionary<long, LoginDao.LoginEntry>();
        private readonly ConcurrentDictionary<string, LoginDao.LoginEntry> usernameCache = new ConcurrentDictionary<string, LoginDao.LoginEntry>();
        private readonly ConcurrentDictionary<LinkedAccount.AccountType, ConcurrentDictionary<string, LoginDao.LoginEntry>> linkedAccountCache = 
            new ConcurrentDictionary<LinkedAccount.AccountType, ConcurrentDictionary<string, LoginDao.LoginEntry>>();

        public LoginDaoCached(LoginDao dao)
        {
            this.dao = dao;
            foreach (LinkedAccount.AccountType type in Enum.GetValues(typeof(LinkedAccount.AccountType)))
            {
                linkedAccountCache.TryAdd(type, new ConcurrentDictionary<string, LoginDao.LoginEntry>());
            }
        }

        private void Cache(LoginDao.LoginEntry account)
        {
            cache.AddOrUpdate(account.AccountId, account, (k, v) => account);
            usernameCache.AddOrUpdate(account.Username, account, (k, v) => account);
            foreach (LinkedAccount linkedAccount in account.LinkedAccounts)
            {
                linkedAccountCache[linkedAccount.Type].AddOrUpdate(linkedAccount.Id, account, (k, v) => account);
            }
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

        public List<LoginDao.LoginEntry> FindRegex(string username)
        {
            if (dao is LoginMockDao)
            {
                Regex regex = new Regex(Regex.Escape(username));
                return usernameCache
                    .Where(x => regex.IsMatch(x.Key))
                    .Select(x => x.Value)
                    .ToList();
            }
            
            List<LoginDao.LoginEntry> daoEntries = dao.FindRegex(username);
            daoEntries.ForEach(Cache);
            return daoEntries;
        }

        public void Save(LoginDao.LoginEntry entry)
        {
            dao.Save(entry);
            Cache(entry);
        }
        
        public LoginDao.LoginEntry FindByLinkedAccount(LinkedAccount linkedAccount)
        {
            if (linkedAccountCache[linkedAccount.Type].TryGetValue(linkedAccount.Id, out var account))
                return account;
            
            var nonCachedAccount = dao.FindByLinkedAccount(linkedAccount);
            if (nonCachedAccount != null)
            {
                Cache(nonCachedAccount);
            }
            return nonCachedAccount;
        }
    }
}