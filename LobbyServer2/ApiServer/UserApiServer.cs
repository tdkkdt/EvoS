using System;
using System.Security.Claims;
using EvoS.DirectoryServer;
using EvoS.DirectoryServer.Account;
using EvoS.Framework;
using EvoS.Framework.Auth;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CentralServer.ApiServer;

public class UserApiServer : ApiServer
{
    public UserApiServer()
        : base(
            LogManager.GetLogger(typeof(UserApiServer)),
            EvosConfiguration.GetUserApiKey(),
            EvosConfiguration.GetUserApiPort(),
            EvosAuth.Context.USER_API)
    {
    }

    protected override void ConfigureApp(WebApplication app)
    {
        app.MapPost("/api/login", Login).AllowAnonymous();
        app.MapPost("/api/register", Register).AllowAnonymous();
        app.MapGet("/api/lobby/status", StatusController.GetSimpleStatus).AllowAnonymous();
        app.MapGet("/api/ticket", GetTicket).RequireAuthorization();
        app.UseAuthorization();
    }
    
    protected IResult Register(HttpContext httpContext, [FromBody] AuthInfo authInfo)
    {
        log.Info($"Registering via api: {authInfo.UserName}");
        try
        {
            LoginManager.Register(authInfo);
        }
        catch (ArgumentException e)
        {
            log.Info($"Cannot register {authInfo.UserName} with given username and/or password", e);
            return Results.BadRequest(new ErrorResponseModel{ message = e.Message });
        }
        catch (Exception e)
        {
            log.Error($"Failed to register {authInfo.UserName}", e);
            return Results.Problem(null, null, StatusCodes.Status500InternalServerError);
        }

        return Login(httpContext, authInfo);
    }

    public static IResult GetTicket(ClaimsPrincipal user)
    {
        if (!EvosConfiguration.GetAllowTicketAuth())
        {
            return Results.StatusCode(StatusCodes.Status501NotImplemented);
        }
        
        EvosAuth.TokenData tokenData = EvosAuth.GetTokenData(user);
        PersistedAccountData account = DB.Get().AccountDao.GetAccount(tokenData.AccountId);
        if (account is null)
        {
            return Results.NotFound();
        }

        AuthTicket authTicket = new AuthTicket(EvosAuth.GenerateToken(EvosAuth.Context.TICKET_AUTH, tokenData), account);
        return new XmlResult(authTicket.AsXML());
    }

}