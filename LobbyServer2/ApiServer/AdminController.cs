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
        
        public static event Action<long, PauseQueueModel> OnAdminPauseQueue = delegate {};
        
        public class PauseQueueModel
        {
            public bool Paused { get; set; }
        }
        
        public static IResult PauseQueue([FromBody] PauseQueueModel data, ClaimsPrincipal user)
        {
            if (!ValidateAdmin(user, out IResult error, out long adminAccountId, out string adminHandle))
            {
                return error;
            }
            MatchmakingManager.Enabled = !data.Paused;
            OnAdminPauseQueue(adminAccountId, data);
            return Results.Ok();
        }

        public class BroadcastModel
        {
            public string Msg { get; set; }
        }
        
        public static IResult Broadcast([FromBody] BroadcastModel data, ClaimsPrincipal user)
        {
            if (!ValidateAdmin(user, out IResult error, out long adminAccountId, out string adminHandle))
            {
                return error;
            }
            log.Info($"Broadcast {adminHandle}: {data.Msg}");
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
        
        public class PenaltyInfo
        {
            public long accountId { get; set; }
            public int durationMinutes { get; set; }
            public string description { get; set; }
        }
        
        public static IResult MuteUser([FromBody] PenaltyInfo data, ClaimsPrincipal user)
        {
            if (!ValidateAdmin(user, out IResult error, out long adminAccountId, out string adminHandle)
                || !Validate(data, out error, out PersistedAccountData account))
            {
                return error;
            }

            string logString = data.durationMinutes > 0
                ? $"MUTE {account.Handle} for {TimeSpan.FromMinutes(data.durationMinutes)}"
                : $"UNMUTE {account.Handle}";
            log.Info($"API {logString} by {adminHandle} ({adminAccountId}): {data.description}");
            bool success = AdminManager.Get().Mute(data.accountId, TimeSpan.FromMinutes(data.durationMinutes), adminHandle, data.description);
            return success ? Results.Ok() : Results.Problem();
        }
        
        public static IResult BanUser([FromBody] PenaltyInfo data, ClaimsPrincipal user)
        {
            if (!ValidateAdmin(user, out IResult error, out long adminAccountId, out string adminHandle)
                || !Validate(data, out error, out PersistedAccountData account))
            {
                return error;
            }

            string logString = data.durationMinutes > 0
                ? $"BAN {account.Handle} for {TimeSpan.FromMinutes(data.durationMinutes)}"
                : $"UNBAN {account.Handle}";
            log.Info($"API {logString} by {adminHandle} ({adminAccountId}): {data.description}");
            bool success = AdminManager.Get().Ban(data.accountId, TimeSpan.FromMinutes(data.durationMinutes), adminHandle, data.description);
            return success ? Results.Ok() : Results.Problem();
        }

        private static bool ValidateAdmin(
            ClaimsPrincipal user,
            out IResult error,
            out long adminAccountId,
            out string adminHandle)
        {
            error = null;
            adminAccountId = 0;
            adminHandle = user.FindFirstValue(ClaimTypes.Name);
            if (adminHandle == null || !long.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out adminAccountId))
            {
                error = Results.Unauthorized();
                return false;
            }

            return true;
        }

        private static bool Validate(
            PenaltyInfo data,
            out IResult error,
            out PersistedAccountData account)
        {
            error = null;
            account = null;
            if (data.durationMinutes < 0)
            {
                error = Results.BadRequest();
                return false;
            }
            account = DB.Get().AccountDao.GetAccount(data.accountId);
            if (account == null)
            {
                error = Results.NotFound();
                return false;
            }

            return true;
        }
    }
}