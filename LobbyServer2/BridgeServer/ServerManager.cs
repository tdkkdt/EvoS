using System.Collections.Generic;
using CentralServer.LobbyServer.Matchmaking;
using log4net;

namespace CentralServer.BridgeServer
{
    public static class ServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerManager));
        
        private static Dictionary<string, BridgeServerProtocol> ServerPool = new Dictionary<string, BridgeServerProtocol>();

        public static void AddServer(BridgeServerProtocol gameServer)
        {
            ServerPool.Add(gameServer.ProcessCode, gameServer);

            log.Info($"New game server connected with address {gameServer.Address}:{gameServer.Port}");
            MatchmakingManager.Update();
        }

        public static void RemoveServer(string processCode)
        {
            ServerPool.Remove(processCode);
            log.Info($"Game server disconnected");
        }

        public static BridgeServerProtocol GetServer()
        {
            lock (ServerPool)
            {
                foreach (BridgeServerProtocol server in ServerPool.Values)
                {
                    if (server.IsAvailable())
                    {
                        server.ReserveForGame();
                        return server;
                    }
                }
            }
            
            return null;
        }

        public static bool IsAnyServerAvailable()
        {
            foreach (BridgeServerProtocol server in ServerPool.Values)
            {
                if (server.IsAvailable()) return true;
            }
            return false;
        }
    }
}
