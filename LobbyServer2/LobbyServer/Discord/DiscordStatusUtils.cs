using CentralServer.LobbyServer.Session;

namespace CentralServer.LobbyServer.Discord
{
    public static class DiscordStatusUtils
    {
        public static string BuildPlayerCountSummary(Status status)
        {
            return $"{status.totalPlayers} player{(status.totalPlayers != 1 ? "s" : "")} online, " +
                   $"{status.inQueue} in queue, {status.inGame} in game";
        }

        public static Status GetStatus()
        {
            int inGame = 0;
            int inQueue = 0;
            int online = 0;
            foreach (long accountId in SessionManager.GetOnlinePlayers())
            {
                LobbyServerProtocol conn = SessionManager.GetClientConnection(accountId);
                if (conn == null) continue;
                online++;
                if (conn.IsInQueue()) inQueue++;
                else if (conn.IsInGame()) inGame++;
            }

            Status status = new Status { totalPlayers = online, inGame = inGame, inQueue = inQueue };
            return status;
        }

        public struct Status
        {
            public int totalPlayers;
            public int inQueue;
            public int inGame;
        }
    }
}