using System;
using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;

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
            
            LoginDao.LoginEntry loginEntry = DB.Get().LoginDao.Find(username.ToLower());
            return loginEntry?.AccountId ?? 0;
        }

        public static string GetHandle(long accountId)
        {
            return DB.Get().AccountDao.GetAccount(accountId)?.Handle ?? "UNKNOWN";
        }

        public static string GameIdString(LobbyGameInfo gameInfo)
        {
            return gameInfo != null
                ? $"{new DateTime(gameInfo.CreateTimestamp):yyyy_MM_dd__HH_mm_ss}"
                : "N/A";
        }
    }
}