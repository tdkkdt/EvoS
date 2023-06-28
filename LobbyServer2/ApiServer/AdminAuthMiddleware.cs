using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;
using log4net;
using Microsoft.AspNetCore.Http;

namespace CentralServer.ApiServer;

public class AdminAuthMiddleware
{
    private static readonly ILog log = LogManager.GetLogger(typeof(AdminAuthMiddleware));
    
    private readonly RequestDelegate _next;

    public AdminAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
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
        await _next(httpContext);
    }
}