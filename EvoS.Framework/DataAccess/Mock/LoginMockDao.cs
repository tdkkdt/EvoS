using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.DataAccess.Mock
{
    public class LoginMockDao: LoginDao
    {
        public LoginDao.LoginEntry FindBySteamId(ulong steamId)
        {
            return null;
        }

        public void UpdateHash(LoginDao.LoginEntry entry, string hash)
        {
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

        void LoginDao.Save(LoginDao.LoginEntry entry)
        {
        }
    }
}