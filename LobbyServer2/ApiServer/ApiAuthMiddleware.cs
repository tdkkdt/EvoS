using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EvoS.DirectoryServer;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;
using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CentralServer.ApiServer;

public class ApiAuthMiddleware
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ApiAuthMiddleware));
    
    private readonly RequestDelegate _next;

    public ApiAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        Endpoint endpoint = httpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() == null)
        {
            EvosAuth.TokenData tokenData = EvosAuth.GetTokenData(httpContext.User);
            if (!ValidateTokenData(httpContext, tokenData))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("");
                return;
            }
        }
        
        PopulateRoles(httpContext);
        await _next(httpContext);
    }

    private static bool ValidateTokenData(HttpContext httpContext, EvosAuth.TokenData tokenData)
    {
        return tokenData is not null && tokenData.IpAddress.Equals(httpContext.Connection.RemoteIpAddress);
    }
    
    private static void PopulateRoles(HttpContext httpContext)
    {
        try
        {
            if (httpContext.User.Identity != null
                && httpContext.User.Identity.IsAuthenticated
                && long.TryParse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out long accountId))
            {
                PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
                if (account != null && account.AccountComponent.AppliedEntitlements.ContainsKey("DEVELOPER_ACCESS"))
                {
                    httpContext.User.AddIdentity(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "api_readonly"),
                        new Claim(ClaimTypes.Role, "api_admin"),
                    }));
                }
            }
        }
        catch (Exception e)
        {
            log.Error(e);
        }
    }
}