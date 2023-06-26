using System;
using System.Security.Claims;
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
        
        public class UserModel
        {
            public long AccountId { get; set; }
            public int DurationMinutes { get; set; }
            public string Description { get; set; }
        }
        
        public static IResult MuteUser([FromBody] UserModel data, ClaimsPrincipal user)
        {
            if (data.DurationMinutes <= 0)
            {
                return Results.BadRequest();
            }
            string adminHandle = user.FindFirstValue(ClaimTypes.Name);
            if (adminHandle == null || !long.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out long adminAccountId))
            {
                return Results.Unauthorized();
            }
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(data.AccountId);
            if (account == null)
            {
                return Results.NotFound();
            }
            
            log.Info($"MUTE {account.Handle} for {TimeSpan.FromMinutes(data.DurationMinutes)} by {adminHandle} ({adminAccountId}): {data.Description}");
            account.AdminComponent.Mute(TimeSpan.FromMinutes(data.DurationMinutes), adminHandle, data.Description);
            DB.Get().AccountDao.UpdateAccount(account);
            return Results.Ok();
        }
    }
}