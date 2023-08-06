using System.Collections.Generic;
using EvoS.Framework.Auth;
using EvoS.Framework.DataAccess.Daos;

namespace EvoS.Framework.DataAccess.Mock
{
    public class LoginMockDao: LoginDao
    {
        public LoginDao.LoginEntry FindBySteamId(ulong steamId)
        {
            return null;
        }

        public void UpdateSteamId(LoginDao.LoginEntry entry, ulong newSteamId)
        {   
        }

        LoginDao.LoginEntry LoginDao.Find(string username)
        {
            return null;
        }

        LoginDao.LoginEntry LoginDao.Find(long accountId)
        {
            return null;
        }

        List<LoginDao.LoginEntry> LoginDao.FindRegex(string username)
        {
            return new List<LoginDao.LoginEntry>();
        }

        public LoginDao.LoginEntry FindByLinkedAccount(LinkedAccount account)
        {
            return null;
        }

        void LoginDao.Save(LoginDao.LoginEntry entry)
        {
        }
    }
}