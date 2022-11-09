using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;

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

        void LoginDao.Save(LoginDao.LoginEntry entry)
        {
        }
    }
}