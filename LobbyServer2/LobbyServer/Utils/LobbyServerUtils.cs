using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;

namespace CentralServer.LobbyServer.Utils
{
    public static class LobbyServerUtils
    {
        public static long ResolveAccountId(long accountId, string handle)
        {
            if (accountId != 0)
            {
                return accountId;
            }
            int hashPos = handle.IndexOf('#');
            if (hashPos < 0)
            {
                return 0;
            }

            string username = handle.Substring(0, hashPos);
            LoginDao.LoginEntry loginEntry = DB.Get().LoginDao.Find(username);
            return loginEntry?.AccountId ?? 0;
        }
    }
}