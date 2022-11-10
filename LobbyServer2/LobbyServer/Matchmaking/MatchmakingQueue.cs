using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CentralServer.BridgeServer;
using CentralServer.LobbyServer.Gamemode;
using CentralServer.LobbyServer.Group;
using EvoS.Framework;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Network.Static;
using log4net;
using WebSocketSharp;

namespace CentralServer.LobbyServer.Matchmaking
{
    class MatchmakingQueue
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MatchmakingQueue));
        
        Dictionary<string, LobbyGameInfo> Games = new Dictionary<string, LobbyGameInfo>();
        SynchronizedCollection<long> QueuedGroups = new SynchronizedCollection<long>();
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

        public LobbyMatchmakingQueueInfo AddGroup(long groupId)
        {
            QueuedGroups.Add(groupId);
            MatchmakingQueueInfo.QueuedPlayers = GetPlayerCount();

            return MatchmakingQueueInfo;
        }

        public int GetPlayerCount()
        {
            return QueuedGroups
                .Select(GroupManager.GetGroup)
                .Sum(group => group?.Members.Count ?? 0);
        }

        public void Update()
        {
            log.Info($"{GetPlayerCount()} players in {GameType} queue");

            foreach (GameSubType subType in MatchmakingQueueInfo.GameConfig.SubTypes)
            {
                // full greed for now
                lock (GroupManager.Lock)
                {
                    List<GroupInfo> groups = QueuedGroups
                        .Select(GroupManager.GetGroup)
                        .Where(group => group != null)
                        .ToList();
                    List<GroupInfo> teamA = new List<GroupInfo>();
                    List<GroupInfo> teamB = new List<GroupInfo>();
                    int teamANum = 0;
                    int teamBNum = 0;
                    foreach (GroupInfo group in groups)
                    {
                        if (teamANum + group.Members.Count <= subType.TeamAPlayers)
                        {
                            teamANum += group.Members.Count;
                            teamA.Add(group);
                        }
                        else if (teamBNum + group.Members.Count <= subType.TeamBPlayers)
                        {
                            teamBNum += group.Members.Count;
                            teamB.Add(group);
                        }
                        if (teamANum == subType.TeamAPlayers && teamBNum == subType.TeamBPlayers)
                        {
                            break;
                        }
                    }

                    if (teamANum == subType.TeamAPlayers && teamBNum == subType.TeamBPlayers)
                    {
                        if (!CheckGameServerAvailable())
                        {
                            log.Warn("No available game server to start a match");
                            return;
                        }
                        foreach (GroupInfo group in teamA)
                        {
                            QueuedGroups.Remove(group.GroupId);
                        }
                        foreach (GroupInfo group in teamB)
                        {
                            QueuedGroups.Remove(group.GroupId);
                        }
                        MatchmakingManager.StartGame(
                            teamA.SelectMany(group => group.Members).ToList(), 
                            teamB.SelectMany(group => group.Members).ToList(), 
                            GameType);
                    }
                }
            }
        }

        public bool CheckGameServerAvailable()
        {
            if (ServerManager.IsAnyServerAvailable()) return true;

            // If there is no game server already connected, we check if we can launch one
            string gameServer = EvosConfiguration.GetGameServerExecutable();
            if (gameServer.IsNullOrEmpty()) return false;
            
            // TODO: this will start a new game server every time the queue is run, which can cause multiple server that aren't needed to start
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
                    log.Error("Failed to start a game server", e);
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
