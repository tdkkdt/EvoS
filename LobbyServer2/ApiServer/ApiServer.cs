using System;
using System.Security.Claims;
using EvoS.DirectoryServer;
using EvoS.DirectoryServer.Account;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;
using log4net;
using log4net.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebSocketSharp;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace CentralServer.ApiServer;

public abstract class ApiServer
{
    protected readonly ILog log;
    private readonly string apiKey;
    private readonly int apiPort;
    private readonly EvosAuth.Context authContext;

    protected ApiServer(ILog log, string apiKey, int apiPort, EvosAuth.Context authContext)
    {
        this.log = log;
        this.apiKey = apiKey;
        this.apiPort = apiPort;
        this.authContext = authContext;
    }

    public WebApplication Init()
    {
        if (CollectionUtilities.IsNullOrEmpty(apiKey))
        {
            log.Info($"{authContext} api server is not enabled");
            return null;
        }
        
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddControllersWithViews();
        builder.Logging.ClearProviders();
        builder.Logging.AddLog4Net(new Log4NetProviderOptions("log4net.xml")
        { 
            LogLevelTranslator = new CustomLogLevelTranslator(),
        });
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader();
                });
        });
        builder.Services.AddAuthentication()
            .AddJwtBearer(o =>
            {
                o.SecurityTokenValidators.Clear();
                o.SecurityTokenValidators.Add(new EvosSecurityTokenValidator(authContext));
            });
        ConfigureBuilder(builder);
        WebApplication app = builder.Build();
        
        app.Use(async (context, next) =>
        {
            log.Debug($"API call: {context.User.FindFirstValue(ClaimTypes.Name) ?? "<anon>"} {authContext} " +
                      $"{context.Request.Method} " +
                      $"{context.Request.Path}{context.Request.QueryString}");
            await next.Invoke();
        });
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        }); 
        app.UseCors();
        app.UseMiddleware<ApiAuthMiddleware>();
        
        ConfigureApp(app);
        
        string url = $"http://localhost:{apiPort}";
        _ = app.RunAsync(url);
        
        log.Info($"Started {authContext} api server at {url}");
        return app;
    }

    protected virtual void ConfigureBuilder(WebApplicationBuilder builder)
    {
        
    }

    protected abstract void ConfigureApp(WebApplication app);

    protected virtual bool LoginFilter(HttpContext httpContext, AuthInfo authInfo, PersistedAccountData account)
    {
        return true;
    }

    protected IResult Login(HttpContext httpContext, [FromBody] AuthInfo authInfo)
    {
        log.Info($"Attempt to login");
        if (authInfo.UserName.IsNullOrEmpty() || authInfo._Password.IsNullOrEmpty())
        {
            log.Info($"Attempt to login for api access without credentials");
            return Results.Unauthorized();
        }
        long accountId;
        try
        {
            accountId = LoginManager.Login(authInfo);
        }
        catch (Exception _)
        {
            log.Info($"Failed to authorize {authInfo.UserName} for {authContext} access");
            return Results.Unauthorized();
        }
        PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
        if (!LoginFilter(httpContext, authInfo, account))
        {
            log.Info($"{authInfo.UserName} attempted to get {authContext} access");
            return Results.Unauthorized();
        }
        string token = EvosAuth.GenerateToken(
            authContext,
            new EvosAuth.TokenData(accountId, httpContext.Connection.RemoteIpAddress, account.Handle));
        log.Info($"{authInfo.UserName} logged in for {authContext} access");
        return Results.Ok(new LoginResponseModel
        {
            handle = account.Handle,
            token = token,
            banner = account.AccountComponent.SelectedForegroundBannerID
        });
    }
        
    public struct LoginResponseModel
    {
        public string handle { get; set; }
        public string token { get; set; }
        public long banner { get; set; }
    }
    
    public class CustomLogLevelTranslator : ILog4NetLogLevelTranslator
    {
        public Level TranslateLogLevel(LogLevel logLevel, Log4NetProviderOptions options) {
            return logLevel switch {
                LogLevel.Critical    => Level.Critical,
                LogLevel.Error       => Level.Error,
                LogLevel.Warning     => Level.Warn,
                LogLevel.Information => Level.Debug,
                LogLevel.Debug       => Level.Debug,
                LogLevel.Trace       => Level.Debug,
                _ => Level.Debug,
            };
        }
    }
}