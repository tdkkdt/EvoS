using System;
using System.Security.Claims;
using CentralServer.LobbyServer;
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
    public static class AdminController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminController));
        
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

        public struct PlayerDetails
        {
            public CommonController.Player player { get; set; }

            public static PlayerDetails Of(PersistedAccountData acc)
            {
                return new PlayerDetails
                {
                    player = CommonController.Player.Of(acc)
                };
            }
        }

        public static IResult GetUser(long accountId)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
            if (account == null)
            {
                return Results.NotFound();
            }

            return Results.Json(PlayerDetails.Of(account));
        }
        
        public class UserModel
        {
            public long AccountId { get; set; }
            public int DurationMinutes { get; set; }
            public string Description { get; set; }
        }
        
        public static IResult MuteUser([FromBody] UserModel data, ClaimsPrincipal user)
        {
            if (!Validate(data, user, out IResult error, out PersistedAccountData account, out long adminAccountId, out string adminHandle))
            {
                return error;
            }

            string logString = data.DurationMinutes > 0
                ? $"MUTE {account.Handle} for {TimeSpan.FromMinutes(data.DurationMinutes)}"
                : $"UNMUTE {account.Handle}";
            log.Info($"API {logString} by {adminHandle} ({adminAccountId}): {data.Description}");
            bool success = AdminManager.Get().Mute(data.AccountId, TimeSpan.FromMinutes(data.DurationMinutes), adminHandle, data.Description);
            return success ? Results.Ok() : Results.Problem();
        }
        
        public static IResult BanUser([FromBody] UserModel data, ClaimsPrincipal user)
        {
            if (!Validate(data, user, out IResult error, out PersistedAccountData account, out long adminAccountId, out string adminHandle))
            {
                return error;
            }

            string logString = data.DurationMinutes > 0
                ? $"BAN {account.Handle} for {TimeSpan.FromMinutes(data.DurationMinutes)}"
                : $"UNBAN {account.Handle}";
            log.Info($"API {logString} by {adminHandle} ({adminAccountId}): {data.Description}");
            bool success = AdminManager.Get().Ban(data.AccountId, TimeSpan.FromMinutes(data.DurationMinutes), adminHandle, data.Description);
            return success ? Results.Ok() : Results.Problem();
        }

        private static bool Validate(
            UserModel data,
            ClaimsPrincipal user,
            out IResult error,
            out PersistedAccountData account,
            out long adminAccountId,
            out string adminHandle)
        {
            error = null;
            account = null;
            adminAccountId = 0;
            adminHandle = null;
            if (data.DurationMinutes < 0)
            {
                error = Results.BadRequest();
                return false;
            }
            adminHandle = user.FindFirstValue(ClaimTypes.Name);
            if (adminHandle == null || !long.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out adminAccountId))
            {
                error = Results.Unauthorized();
                return false;
            }
            account = DB.Get().AccountDao.GetAccount(data.AccountId);
            if (account == null)
            {
                error = Results.NotFound();
                return false;
            }

            return true;
        }
    }
}