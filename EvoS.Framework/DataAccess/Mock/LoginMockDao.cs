using System.Collections.Generic;
using EvoS.Framework.DataAccess.Daos;

namespace EvoS.Framework.DataAccess.Mock
{
    public class LoginMockDao: LoginDao
    {
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

        void LoginDao.Save(LoginDao.LoginEntry entry)
        {
        }
    }
}