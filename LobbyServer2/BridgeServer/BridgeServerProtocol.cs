using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralServer.LobbyServer;
using CentralServer.LobbyServer.Gamemode;
using CentralServer.LobbyServer.Matchmaking;
using CentralServer.LobbyServer.Session;
using CentralServer.LobbyServer.Utils;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Misc;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using log4net;
using WebSocketSharp;

namespace CentralServer.BridgeServer
{
    public class BridgeServerProtocol : GameServerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BridgeServerProtocol));

        public string Address;
        public int Port;
        private LobbySessionInfo SessionInfo;
        private MatchOrchestrator Orchestrator;
        public DateTime StopTime { private set; get; }

        public string URI => "ws://" + Address + ":" + Port;
        public string BuildVersion => SessionInfo?.BuildVersion ?? "";
        public bool IsPrivate { get; private set; }

        public ServerGameMetrics GameMetrics { get; private set; } = new ServerGameMetrics();
        public LobbyGameSummary GameSummary { get; private set; }

        protected override string GetConnContext()
        {
            return $"S {Address}:{Port}";
        }

        public BridgeServerProtocol()
        {
            Orchestrator = new MatchOrchestrator(this);

            RegisterHandler<RegisterGameServerRequest>(HandleRegisterGameServerRequest);
            RegisterHandler<ServerGameSummaryNotification>(HandleServerGameSummaryNotification);
            RegisterHandler<PlayerDisconnectedNotification>(HandlePlayerDisconnectedNotification);
            RegisterHandler<ServerGameMetricsNotification>(HandleServerGameMetricsNotification);
            RegisterHandler<ServerGameStatusNotification>(HandleServerGameStatusNotification);
            RegisterHandler<MonitorHeartbeatNotification>(HandleMonitorHeartbeatNotification);
            RegisterHandler<LaunchGameResponse>(HandleLaunchGameResponse);
            RegisterHandler<JoinGameServerResponse>(HandleJoinGameServerResponse);
            RegisterHandler<ReconnectPlayerResponse>(HandleReconnectPlayerResponse);
        }

        private void HandleRegisterGameServerRequest(RegisterGameServerRequest request, int callbackId)
        {
            string data = request.SessionInfo.ConnectionAddress;
            Address = data.Split(":")[0];
            Port = Convert.ToInt32(data.Split(":")[1]);
            SessionInfo = request.SessionInfo;
            ProcessCode = SessionInfo.ProcessCode;
            Name = SessionInfo.UserName ?? "ATLAS";
            IsPrivate = request.isPrivate;
            ServerManager.AddServer(this);

            Send(new RegisterGameServerResponse
            {
                Success = true
            },
                callbackId);
        }

        private void HandleServerGameSummaryNotification(ServerGameSummaryNotification notify)
        {
            LobbyGameSummary gameSummary = notify.GameSummary;
            if (gameSummary == null)
            {
                GameInfo.GameResult = GameResult.TieGame;
                gameSummary = new LobbyGameSummary();
            }
            else
            {
                GameInfo.GameResult = gameSummary.GameResult;
            }

            GameSummary = gameSummary;
            log.Info($"Game {GameInfo?.Name} at {GameSummary.GameServerAddress} finished " +
                     $"({GameSummary.NumOfTurns} turns), " +
                     $"{GameSummary.GameResult} {GameSummary.TeamAPoints}-{GameSummary.TeamBPoints}");

            try
            {
                GameSummary.BadgeAndParticipantsInfo = AccoladeUtils.ProcessGameSummary(GameSummary);
            }
            catch (Exception ex)
            {
                log.Error("Failed to process game summary", ex);
            }

            DB.Get().MatchHistoryDao.Save(MatchHistoryDao.MatchEntry.Cons(GameInfo, GameSummary));

            ServerGameStatus = GameStatus.Stopped;
            StopTime = DateTime.UtcNow.Add(TimeSpan.FromSeconds(8));

            _ = Orchestrator.FinalizeGame(GameSummary);
        }

        private void HandlePlayerDisconnectedNotification(PlayerDisconnectedNotification request)
        {
            log.Info($"{LobbyServerUtils.GetHandle(request.PlayerInfo.AccountId)} left game {GameInfo?.GameServerProcessCode}");

            foreach (LobbyServerProtocol client in GetClients())
            {
                if (client.AccountId == request.PlayerInfo.AccountId)
                {
                    client.LeaveServer(this);
                    break;
                }
            }

            LobbyServerPlayerInfo playerInfo = GetPlayerInfo(request.PlayerInfo.AccountId);
            if (playerInfo != null)
            {
                playerInfo.ReplacedWithBots = true;
            }
            
            QueuePenaltyManager.IssueQueuePenalties(request.PlayerInfo.AccountId, this);
        }

        private void HandleServerGameMetricsNotification(ServerGameMetricsNotification request)
        {
            GameMetrics = request.GameMetrics;
            log.Info($"Game {GameInfo?.Name} Turn {request.GameMetrics?.CurrentTurn}, " +
                     $"{request.GameMetrics?.TeamAPoints}-{request.GameMetrics?.TeamBPoints}, " +
                     $"frame time: {request.GameMetrics?.AverageFrameTime}");
        }

        private void HandleServerGameStatusNotification(ServerGameStatusNotification request)
        {
            log.Info($"Game {GameInfo?.Name} {request.GameStatus}");

            ServerGameStatus = request.GameStatus;

            if (ServerGameStatus == GameStatus.Stopped)
            {
                foreach (LobbyServerProtocol client in GetClients())
                {
                    if (!client.LeaveServer(this))
                    {
                        continue;
                    }

                    if (GameInfo != null)
                    {
                        //Unready people when game is finisht
                        ForceMatchmakingQueueNotification forceMatchmakingQueueNotification =
                            new ForceMatchmakingQueueNotification()
                            {
                                Action = ForceMatchmakingQueueNotification.ActionType.Leave,
                                GameType = GameInfo.GameConfig.GameType
                            };
                        client.Send(forceMatchmakingQueueNotification);
                    }
                }
            }
        }

        private void HandleMonitorHeartbeatNotification(MonitorHeartbeatNotification notify)
        {

        }

        private void HandleLaunchGameResponse(LaunchGameResponse response)
        {
            log.Info(
                $"Game {GameInfo?.Name} launched ({response.GameServerAddress}, {response.GameInfo?.GameStatus}) with {response.GameInfo?.ActiveHumanPlayers} players");
        }

        private void HandleJoinGameServerResponse(JoinGameServerResponse response)
        {
            log.Info(
                $"Player {response.PlayerInfo?.Handle} {response.PlayerInfo?.AccountId} {response.PlayerInfo?.CharacterType} " +
                $"joined {GameInfo?.Name} ({response.GameServerProcessCode})");
        }

        private void HandleReconnectPlayerResponse(ReconnectPlayerResponse response)
        {
            if (!response.Success)
            {
                log.Error("Reconnecting player is not found on the server");
            }
        }

        protected override void HandleClose(CloseEventArgs e)
        {
            QueuePenaltyManager.CapQueuePenalties(this);
            UnregisterAllHandlers();
            ServerManager.RemoveServer(ProcessCode);
        }

        public bool IsAvailable()
        {
            return ServerGameStatus == GameStatus.None && !IsPrivate && IsConnected;
        }

        public void ReserveForGame()
        {
            ServerGameStatus = GameStatus.Assembling;
            // TODO release if game did not start?
        }

        public async Task StartGameAsync(List<long> teamA, List<long> teamB, GameType gameType, GameSubType gameSubType)
        {
            await Orchestrator.StartGameAsync(teamA, teamB, gameType, gameSubType);
        }

        public async Task StartCustomGameAsync(LobbyServerTeamInfo teamInfo, LobbyGameInfo gameInfo)
        {
            TeamInfo = teamInfo;
            BuildGameInfoCustomGame(gameInfo);
            await Orchestrator.StartCustomGameAsync();
        }

        public void StartGameForReconnection(long accountId)
        {
            LobbySessionInfo sessionInfo = SessionManager.GetSessionInfo(accountId);
            Send(new ReconnectPlayerRequest
            {
                AccountId = accountId,
                NewSessionId = sessionInfo.SessionToken
            });
        }

        public void StartGame()
        {
            ServerGameStatus = GameStatus.Assembling;
            Dictionary<int, LobbySessionInfo> sessionInfos = TeamInfo.TeamPlayerInfo
                .ToDictionary(
                    playerInfo => playerInfo.PlayerId,
                    playerInfo => SessionManager.GetSessionInfo(playerInfo.AccountId) ?? new LobbySessionInfo());  // fallback for bots TODO something smarter

            foreach (LobbyServerPlayerInfo playerInfo in TeamInfo.TeamPlayerInfo)
            {
                LobbySessionInfo sessionInfo = sessionInfos[playerInfo.PlayerId];
                JoinGameServerRequest request = new JoinGameServerRequest
                {
                    OrigRequestId = 0,
                    GameServerProcessCode = GameInfo.GameServerProcessCode,
                    PlayerInfo = playerInfo,
                    SessionInfo = sessionInfo
                };
                Send(request);
            }

            Send(new LaunchGameRequest()
            {
                GameInfo = GameInfo,
                TeamInfo = TeamInfo,
                SessionInfo = sessionInfos,
                GameplayOverrides = GameConfig.GetGameplayOverrides()
            });
        }

        public bool Send(AllianceMessageBase msg, int originalCallbackId = 0)
        {
            short messageType = BridgeMessageSerializer.GetMessageType(msg);
            if (messageType >= 0)
            {
                Send(messageType, msg, originalCallbackId);
                LogMessage(">", msg);
                return true;
            }
            log.Error($"No sender for {msg.GetType().Name}");
            LogMessage(">X", msg);

            return false;
        }

        private void Send(short msgType, AllianceMessageBase msg, int originalCallbackId = 0)
        {
            Send(BridgeMessageSerializer.SerializeMessage(msgType, msg, originalCallbackId));
        }

        public void BuildGameInfo(GameType gameType, GameSubType gameMode)
        {
            GameInfo = new LobbyGameInfo
            {
                AcceptedPlayers = TeamInfo.TeamPlayerInfo.Count(p => p.IsReady),
                AcceptTimeout = new TimeSpan(0, 0, 0),
                SelectTimeout = TimeSpan.FromSeconds(30),
                LoadoutSelectTimeout = TimeSpan.FromSeconds(30),
                ActiveHumanPlayers = TeamInfo.TeamPlayerInfo.Count(p => p.IsHumanControlled),
                ActivePlayers = TeamInfo.TeamPlayerInfo.Count,
                CreateTimestamp = DateTime.UtcNow.Ticks,
                GameConfig = new LobbyGameConfig
                {
                    GameOptionFlags = GameOptionFlag.NoInputIdleDisconnect & GameOptionFlag.NoInputIdleDisconnect,
                    GameServerShutdownTime = -1,
                    GameType = gameType,
                    InstanceSubTypeBit = 1,
                    IsActive = true,
                    Map = MatchmakingQueue.SelectMap(gameMode),
                    ResolveTimeoutLimit = 1600, // TODO ?
                    RoomName = "",
                    Spectators = 0,
                    SubTypes = GameModeManager.GetGameTypeAvailabilities()[gameType].SubTypes,
                    TeamABots = 0,
                    TeamAPlayers = TeamInfo.TeamAPlayerInfo.Count(),
                    TeamBBots = 0,
                    TeamBPlayers = TeamInfo.TeamBPlayerInfo.Count(),
                },
                GameResult = GameResult.NoResult,
                GameServerAddress = this.URI,
                GameServerProcessCode = this.ProcessCode
            };
        }

        public void BuildGameInfoCustomGame(LobbyGameInfo gameinfo)
        {
            GameInfo = gameinfo.Clone();
            GameInfo.ActivePlayers = TeamInfo.TeamPlayerInfo.Count;
            GameInfo.LoadoutSelectTimeout = TimeSpan.FromSeconds(30);
            GameInfo.AcceptedPlayers = TeamInfo.TeamPlayerInfo.Count;
            GameInfo.ActiveHumanPlayers = TeamInfo.TeamPlayerInfo.Count(p => p.IsHumanControlled);
            GameInfo.GameConfig.TeamAPlayers = TeamInfo.TeamAPlayerInfo.Count();
            GameInfo.GameConfig.TeamAPlayers = TeamInfo.TeamAPlayerInfo.Count();
            GameInfo.GameConfig.Spectators = TeamInfo.SpectatorInfo.Count();
            GameInfo.GameServerAddress = this.URI;
            // GameInfo.GameServerProcessCode = this.ProcessCode; // TODO CUSTOM GAMES Actual server process code doesn't match the one on the client and in ServerManager, might cause issues
        }

        public void SetGameStatus(GameStatus status)
        {
            GameInfo.GameStatus = status;
        }

        public override bool UpdateCharacterInfo(long accountId, LobbyCharacterInfo characterInfo, LobbyPlayerInfoUpdate update)
        {
            return Orchestrator.UpdateCharacterInfo(accountId, characterInfo, update);
        }
        
        public override void SetSecondaryCharacter(long accountId, int playerId, CharacterType characterType)
        {
            LobbyServerPlayerInfo lobbyServerPlayerInfo = TeamInfo.TeamPlayerInfo.Find(p => p.PlayerId == playerId);
            if (lobbyServerPlayerInfo is null)
            {
                log.Error($"Failed to set secondary character: {playerId} not found");
                return;
            }
            if (lobbyServerPlayerInfo.AccountId != accountId)
            {
                log.Error($"Failed to set secondary character: {playerId} does not belong to {LobbyServerUtils.GetHandle(accountId)}");
                return;
            }
            lobbyServerPlayerInfo.CharacterInfo = new LobbyCharacterInfo() { CharacterType = characterType };
        }

        public void Shutdown()
        {
            Send(new ShutdownGameRequest());
        }

        public override void DisconnectPlayer(long accountId)
        {
            Send(new DisconnectPlayerRequest
            {
                SessionInfo = SessionManager.GetSessionInfo(accountId),
                PlayerInfo = GetPlayerInfo(accountId),
                GameResult = GameResult.ClientLeft
            });
        }
    }
}