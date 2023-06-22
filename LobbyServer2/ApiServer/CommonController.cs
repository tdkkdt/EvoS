using System;
using System.Collections.Generic;
using System.Linq;
using CentralServer.BridgeServer;
using CentralServer.LobbyServer.Group;
using CentralServer.LobbyServer.Matchmaking;
using CentralServer.LobbyServer.Session;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CentralServer.ApiServer
{

    public static class CommonController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommonController));
        
        public class PauseQueueModel
        {
            public bool Paused { get; set; }
        }
        
        public static IResult PauseQueue([FromBody] PauseQueueModel data)
        {
            MatchmakingManager.Enabled = !data.Paused;
            return Results.Ok();
        }

        public class BroadcastModel
        {
            public string Msg { get; set; }
        }
        
        public static IResult Broadcast([FromBody] BroadcastModel data)
        {
            log.Info($"Broadcast {data.Msg}");
            if (data.Msg.IsNullOrEmpty())
            {
                return Results.BadRequest();
            }
            SessionManager.Broadcast(data.Msg);
            return Results.Ok();
        }
        
        public static IResult GetStatus()
        {
            List<BridgeServerProtocol> servers = ServerManager.GetServers();
            Status status = new Status
            {
                players = SessionManager.GetOnlinePlayers()
                    .Select(id => DB.Get().AccountDao.GetAccount(id))
                    .Select(Player.Of)
                    .ToList(),
                groups = GroupManager.GetGroups()
                    .Select(Group.Of)
                    .ToList(),
                queues = MatchmakingManager.GetQueues()
                    .Select(Queue.Of)
                    .ToList(),
                servers = servers
                    .Select(Server.Of)
                    .ToList(),
                games = servers
                    .Where(s => !s.IsAvailable())
                    .Select(Game.Of)
                    .ToList()
            };
            return Results.Json(status);
        }

        public struct Status
        {
            public List<Player> players { get; set; }
            public List<Group> groups { get; set; }
            public List<Queue> queues { get; set; }
            public List<Server> servers { get; set; }
            public List<Game> games { get; set; }
        }

        public struct Player
        {
            public long accountId { get; set; }
            public string handle { get; set; }
            public int bannerBg { get; set; }
            public int bannerFg { get; set; }

            public static Player Of(PersistedAccountData acc)
            {
                return new Player
                {
                    accountId = acc.AccountId,
                    handle = acc.Handle,
                    bannerBg = acc.AccountComponent.SelectedBackgroundBannerID,
                    bannerFg = acc.AccountComponent.SelectedForegroundBannerID,
                };
            }
        }

        public struct Group
        {
            public long groupId { get; set; }
            public List<long> accountIds { get; set; }

            public static Group Of(GroupInfo g)
            {
                return new Group
                {
                    accountIds = g.Members,
                    groupId = g.GroupId
                };
            }
        }

        public struct Queue
        {
            public string type { get; set; }
            public List<long> groupIds { get; set; }

            public static Queue Of(MatchmakingQueue q)
            {
                return new Queue
                {
                    type = q.GameType.ToString(),
                    groupIds = q.GetQueuesGroups()
                };
            }
        }

        public struct Server
        {
            public string id { get; set; }
            public string name { get; set; }

            public static Server Of(BridgeServerProtocol s)
            {
                return new Server
                {
                    id = s.ID,
                    name = s.Name
                };
            }
        }
        
        public struct Game
        {
            public string id { get; set; }
            public string ts { get; set; }
            public string server { get; set; }
            public List<GamePlayer> teamA { get; set; }
            public List<GamePlayer> teamB { get; set; }
            public string map { get; set; }
            public string status { get; set; }
            public int turn { get; set; }
            public int teamAScore { get; set; }
            public int teamBScore { get; set; }

            public static Game Of(BridgeServerProtocol s)
            {
                return new Game
                {
                    id = s.GetGameInfo.GameServerProcessCode,
                    ts = $"{new DateTime(s.GameInfo.CreateTimestamp):yyyy_MM_dd__HH_mm_ss}",
                    map = s.GetGameInfo.GameConfig.Map,
                    server = s.ID,
                    teamA = s.GetTeamInfo.TeamAPlayerInfo
                        .Select(GamePlayer.Of)
                        .ToList(),
                    teamB = s.GetTeamInfo.TeamBPlayerInfo
                        .Select(GamePlayer.Of)
                        .ToList(),
                    status = s.GameInfo.GameStatus.ToString(),
                    turn = s.GameMetrics.CurrentTurn,
                    teamAScore = s.GameMetrics.TeamAPoints,
                    teamBScore = s.GameMetrics.TeamBPoints,
                };
            }
        }
        
        public struct GamePlayer
        {
            public long accountId { get; set; }
            public string characterType { get; set; }

            public static GamePlayer Of(LobbyServerPlayerInfo playerInfo)
            {
                return new GamePlayer
                {
                    accountId = playerInfo.AccountId,
                    characterType = playerInfo.CharacterType.ToString()
                };
            }
        }
    }
}