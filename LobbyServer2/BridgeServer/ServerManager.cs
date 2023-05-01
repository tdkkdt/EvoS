using System.Collections.Generic;
using CentralServer.LobbyServer.Matchmaking;
using EvoS.Framework.Constants.Enums;
using log4net;

namespace CentralServer.BridgeServer
{
    public static class ServerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServerManager));
        
        private static readonly Dictionary<string, BridgeServerProtocol> ServerPool = new Dictionary<string, BridgeServerProtocol>();

        public static void AddServer(BridgeServerProtocol gameServer)
        {
            lock (ServerPool)
            {
                ServerPool.Add(gameServer.ProcessCode, gameServer);

                log.Info($"New game server connected with address {gameServer.Address}:{gameServer.Port}");
                MatchmakingManager.Update();
            }
        }

        public static void RemoveServer(string processCode)
        {
            lock (ServerPool)
            {
                ServerPool.Remove(processCode);
                log.Info($"Game server disconnected");
            }
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

        public static BridgeServerProtocol GetServerWithPlayer(long accountId)
        {
            lock (ServerPool)
            {
                foreach (BridgeServerProtocol server in ServerPool.Values)
                {
                    if (server.ServerGameStatus == GameStatus.Started) // TODO why started only?
                    {
                        foreach (long player in server.GetPlayers())
                        {
                            if (player.Equals(accountId))
                            {
                                return server;
                            }
                        }
                    }
                }

                return null;
            }
        }

        public static bool IsAnyServerAvailable()
        {
            lock (ServerPool)
            {
                foreach (BridgeServerProtocol server in ServerPool.Values)
                {
                    if (server.IsAvailable()) return true;
                }

                return false;
            }
        }
    }
}
