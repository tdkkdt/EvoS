using System;
using System.Threading;
using CentralServer.BridgeServer;
using CentralServer.LobbyServer;
using EvoS.Framework;
using log4net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CentralServer
{
    public class CentralServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CentralServer));
        
        public static void Main(string[] args)
        {
            int port = EvosConfiguration.GetLobbyServerPort();
            WebSocketServer server = new WebSocketServer(port);
            server.AddWebSocketService<LobbyServerProtocol>("/LobbyGameClientSessionManager");
            server.AddWebSocketService<BridgeServerProtocol>("/BridgeServer");
            server.Log.Level = LogLevel.Debug;

            server.Start();
            log.Info($"Started lobby server on port {port}");
            if (EvosConfiguration.GetGameServerExecutable().IsNullOrEmpty())
            {
                log.Warn("GameServerExecutable not set in settings.yaml. " +
                         "Automatic game server launch is disabled. Game servers can still connect to this lobby");
            }

            while (server.IsListening)
            {
                Thread.Sleep(5000);
            }
            log.Info("Lobby server is not listening, exiting...");
            throw new ApplicationException("Lobby is down");
        }
    }
}
