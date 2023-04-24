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
            string username = hashPos >= 0
                ? handle.Substring(0, hashPos)
                : handle;
            
            LoginDao.LoginEntry loginEntry = DB.Get().LoginDao.Find(username);
            return loginEntry?.AccountId ?? 0;
        }
    }
}