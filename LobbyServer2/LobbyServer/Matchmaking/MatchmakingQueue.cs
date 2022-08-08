using System;
using System.Collections.Generic;
using System.Diagnostics;
using CentralServer.BridgeServer;
using CentralServer.LobbyServer.Gamemode;
using EvoS.Framework;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Logging;
using EvoS.Framework.Network.Static;
using WebSocketSharp;

namespace CentralServer.LobbyServer.Matchmaking
{
    class MatchmakingQueue
    {
        Dictionary<string, LobbyGameInfo> Games = new Dictionary<string, LobbyGameInfo>();
        SynchronizedCollection<LobbyServerProtocolBase> Players = new SynchronizedCollection<LobbyServerProtocolBase>();
        public LobbyMatchmakingQueueInfo MatchmakingQueueInfo;
        GameType GameType => MatchmakingQueueInfo.GameType;
        
        private static int GameID = 0;

        public MatchmakingQueue(GameType gameType)
        {
            MatchmakingQueueInfo = new LobbyMatchmakingQueueInfo()
            {
                QueueStatus = QueueStatus.Idle,
                QueuedPlayers = 0,
                ShowQueueSize = true,
                AverageWaitTime = TimeSpan.FromSeconds(60),
                GameConfig = new LobbyGameConfig()
                {
                    GameType = gameType,
                    SubTypes = GameModeManager.GetGameTypeAvailabilities()[gameType].SubTypes
                }
            };
        }

        public LobbyMatchmakingQueueInfo AddPlayer(LobbyServerProtocolBase connection)
        {
            Players.Add(connection);
            MatchmakingQueueInfo.QueuedPlayers = Players.Count;

            return MatchmakingQueueInfo;
        }

        public void Update()
        {
            RemoveDisconnectedPlayers();
            Log.Print(LogType.Game, $"{Players.Count} players in {GameType} queue");

            foreach (GameSubType subType in MatchmakingQueueInfo.GameConfig.SubTypes)
            {
                int FullGamePlayers = subType.TeamAPlayers + subType.TeamBPlayers;
                if (Players.Count >= FullGamePlayers)
                {
                    if (!CheckGameServerAvailable())
                    {
                        Log.Print(LogType.Error, "No available game server to start a match");
                        return;
                    }

                    List<LobbyServerProtocolBase> clients = new List<LobbyServerProtocolBase>();
                    for (int i = 0; i < FullGamePlayers; i++)
                    {
                        LobbyServerProtocolBase client = Players[i];
                        clients.Add(client);
                    }
                    MatchmakingManager.StartGame(clients, GameType);
                }
            }
        }

        public void RemoveDisconnectedPlayers()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                LobbyServerProtocolBase connection = Players[i];
                if (connection.State != WebSocketState.Open)
                {
                    Log.Print(LogType.Game, $"Removing disconnected player {connection.AccountId} from {GameType} queue");
                    Players.RemoveAt(i--);
                }
            }
        }

        public void RemovePlayers(List<LobbyServerProtocolBase> playerList)
        {
            foreach (LobbyServerProtocolBase player in playerList)
            {
                Players.Remove(player);
            }
        }

        public bool CheckGameServerAvailable()
        {
            if (ServerManager.IsAnyServerAvailable()) return true;

            // If there is no game server already connected, we check if we can launch one
            string gameServer = EvosConfiguration.GetGameServerExecutable();
            if (gameServer.IsNullOrEmpty()) return false;
            
            // TODO: this will start a new game server eveerytime the queue is runned, which can cause multiple server that aren't needed to start
            using (var process = new Process())
            {
                try
                {
                    process.StartInfo = new ProcessStartInfo(gameServer);
                    process.Start();

                    return true;
                }
                catch(Exception e)
                {
                    Log.Print(LogType.Error, e.ToString());
                    return false;
                }   
            }
        }

        public LobbyGameInfo CreateGameInfo()
        {
            GameTypeAvailability gameAvailability = GameModeManager.GetGameTypeAvailabilities()[GameType];

            LobbyGameInfo gameInfo = new LobbyGameInfo
            {
                AcceptedPlayers = gameAvailability.TeamAPlayers + gameAvailability.TeamBPlayers,
                AcceptTimeout = GameType == GameType.Practice ? TimeSpan.FromSeconds(0) : TimeSpan.FromSeconds(5),
                ActiveHumanPlayers = 0,
                ActivePlayers = 0,
                ActiveSpectators = 0,
                CreateTimestamp = DateTime.Now.Ticks,
                GameConfig = new LobbyGameConfig
                {
                    GameOptionFlags = GameOptionFlag.AllowDuplicateCharacters & GameOptionFlag.EnableTeamAIOutput & GameOptionFlag.NoInputIdleDisconnect,
                    GameType = GameType,
                    IsActive = true,
                    RoomName = $"Evos-{GameType.ToString()}-{GameID++}",
                    SubTypes = gameAvailability.SubTypes,
                    TeamAPlayers = gameAvailability.TeamAPlayers,
                    TeamABots = gameAvailability.TeamABots,
                    TeamBPlayers = gameAvailability.TeamBPlayers,
                    TeamBBots = gameAvailability.TeamBBots,
                    Map = SelectMap(gameAvailability),
                    ResolveTimeoutLimit = 1600,
                    Spectators = 0
                },
                GameResult = GameResult.NoResult,
                GameServerAddress = "",
                GameStatus = GameStatus.Assembling,
                GameServerHost = "",
                IsActive = true,
                LoadoutSelectTimeout = GameType==GameType.Practice ? TimeSpan.FromSeconds(0) : TimeSpan.FromSeconds(5),
                SelectTimeout = GameType==GameType.Practice ? TimeSpan.FromSeconds(0) : TimeSpan.FromSeconds(5),
                // TODO: there are more options that may be usefull
            };

            return gameInfo;
        }

        public string SelectMap(GameTypeAvailability gameTypeAvailability)
        {
            List<GameMapConfig> maps = gameTypeAvailability.SubTypes[0].GameMapConfigs;
            Random rand = new Random();
            int index = rand.Next(0, maps.Count);
            return maps[index].Map;
        }

        public void SetGameStatus(string roomName, GameStatus gameStatus)
        {
            Games[roomName].GameStatus = gameStatus;
        }

    }
}
