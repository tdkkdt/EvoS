using System;
using System.Threading;
using System.Threading.Tasks;
using CentralServer.BridgeServer;
using CentralServer.LobbyServer;
using CentralServer.LobbyServer.Chat;
using CentralServer.LobbyServer.Discord;
using EvoS.Framework;
using log4net;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CentralServer
{
    public class CentralServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CentralServer));

        private static WebSocketServer server;

        public static async Task Init(string[] args)
        {
            int port = EvosConfiguration.GetLobbyServerPort();
            server = new WebSocketServer(port);
            server.AddWebSocketService<LobbyServerProtocol>("/LobbyGameClientSessionManager");
            server.AddWebSocketService<BridgeServerProtocol>("/BridgeServer");
            server.Log.Level = LogLevel.Debug;

            ChatManager.Get(); // TODO Dependency injection
            await DiscordManager.Get().Start();
            AdminManager.Get().Start();
            
            server.Start();
            log.Info($"Started lobby server on port {port}");

            var adminApi = new ApiServer.AdminApiServer().Init();
            var userApi = new ApiServer.UserApiServer().Init();

            if (EvosConfiguration.GetGameServerExecutable().IsNullOrEmpty())
            {
                log.Warn("GameServerExecutable not set in settings.yaml. " +
                         "Automatic game server launch is disabled. Game servers can still connect to this lobby");
            }
        }

        public static void MainLoop()
        {
            while (server.IsListening)
            {
                Thread.Sleep(5000);
            }
            
            DiscordManager.Get().Shutdown();
            
            log.Info("Lobby server is not listening, exiting...");
            throw new ApplicationException("Lobby is down");
        }
    }
}
