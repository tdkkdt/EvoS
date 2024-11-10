using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BitFaster.Caching.Lru;
using EvoS.Framework.Auth;
using EvoS.Framework.DataAccess.Mock;

namespace EvoS.Framework.DataAccess.Daos
{
    public class LoginDaoCached: LoginDao
    {
        private readonly LoginDao dao;
        
        private const int Capacity = 2048;
        private readonly FastConcurrentLru<long, LoginDao.LoginEntry> cache = new(Capacity);
        private readonly FastConcurrentLru<string, long> usernameCache = new(Capacity);
        private readonly ConcurrentDictionary<LinkedAccount.AccountType, FastConcurrentLru<string, long>> linkedAccountCache = new();

        public LoginDaoCached(LoginDao dao)
        {
            this.dao = dao;
            foreach (LinkedAccount.AccountType type in Enum.GetValues(typeof(LinkedAccount.AccountType)))
            {
                linkedAccountCache.TryAdd(type, new(Capacity));
            }
        }

        private void Cache(LoginDao.LoginEntry account)
        {
            cache.AddOrUpdate(account.AccountId, account);
            usernameCache.AddOrUpdate(account.Username, account.AccountId);
            foreach (LinkedAccount linkedAccount in account.LinkedAccounts)
            {
                linkedAccountCache[linkedAccount.Type].AddOrUpdate(linkedAccount.Id, account.AccountId);
            }
        }

        public LoginDao.LoginEntry Find(string username)
        {
            if (usernameCache.TryGet(username, out var accountId))
            {
                return Find(accountId);
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
            if (cache.TryGet(accountId, out var account))
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
                    .Select(x => cache.TryGet(x.Value, out var account) ? account : null)
                    .Where(x => x is not null)
                    .ToList();
            }
            
            List<LoginDao.LoginEntry> daoEntries = dao.FindRegex(username);
            daoEntries.ForEach(Cache);
            return daoEntries;
        }

        public List<LoginDao.LoginEntry> FindAll()
        {
            if (dao is LoginMockDao)
            {
                return usernameCache
                    .Select(x => cache.TryGet(x.Value, out var account) ? account : null)
                    .ToList();
            }
            
            List<LoginDao.LoginEntry> daoEntries = dao.FindAll();
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
            if (linkedAccountCache[linkedAccount.Type].TryGet(linkedAccount.Id, out var accountId))
            {
                return Find(accountId);
            }
            
            var nonCachedAccount = dao.FindByLinkedAccount(linkedAccount);
            if (nonCachedAccount != null)
            {
                Cache(nonCachedAccount);
            }
            return nonCachedAccount;
        }
    }
}