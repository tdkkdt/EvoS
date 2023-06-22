using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CentralServer.ApiServer;
using EvoS.DirectoryServer.Account;
using EvoS.Framework;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;
using log4net;
using log4net.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WebSocketSharp;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace CentralServer;

public class AdminServer
{
    private static readonly ILog log = LogManager.GetLogger(typeof(AdminServer));

    private static readonly string TokenIssuer = "AtlasReactor";
    private static readonly string TokenAudience = "AtlasReactor";
    
    private static readonly string EndpointLogin = "/api/login";
    
    public WebApplication Init()
    {
        string apiKey = EvosConfiguration.GetApiKey();
        if (CollectionUtilities.IsNullOrEmpty(apiKey))
        {
            log.Info("Api server is not enabled");
            return null;
        }
        
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllersWithViews();
        builder.Logging.ClearProviders();
        builder.Logging.AddLog4Net(new Log4NetProviderOptions("log4net.xml")
        { 
            LogLevelTranslator = new CustomLogLevelTranslator(),
        });
        builder.Services.AddAuthentication()
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = TokenIssuer,
                    ValidAudience = TokenAudience,
                    IssuerSigningKey = Key(apiKey),
                    // ClockSkew = TimeSpan.Zero,
                };
            });
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("api_readonly", policy => policy.RequireRole("api_readonly"))
            .AddPolicy("api_admin", policy => policy.RequireRole("api_admin"));
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader();
                });
        });
        var app = builder.Build();
        
        app.Use(async (context, next) =>
        {
            log.Info($"API call: {context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "<anon>"} " +
                     $"{context.Request.Method} " +
                     $"{context.Request.Path}{(EndpointLogin.Equals(context.Request.Path) ? string.Empty : context.Request.QueryString)}");
            await next.Invoke();
        });
        
        app.MapPost(EndpointLogin, Login).AllowAnonymous();
        app.MapGet("/api/lobby/status", CommonController.GetStatus).RequireAuthorization("api_readonly");
        app.MapPost("/api/lobby/broadcast", CommonController.Broadcast).RequireAuthorization("api_admin");
        app.MapPut("/api/queue/paused", CommonController.PauseQueue).RequireAuthorization("api_admin");
        
        app.UseCors();
        app.UseAuthorization();
        
        string url = $"http://localhost:{EvosConfiguration.GetApiPort()}";
        _ = app.RunAsync(url);
        
        log.Info($"Started admin server at {url}");
        return app;
    }

    private static SymmetricSecurityKey Key(string key)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    private static IResult Login([FromBody] AuthInfo authInfo)
    {
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
            log.Info($"Failed to authorize {authInfo.UserName} for api access");
            return Results.Unauthorized();
        }
        PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
        if (!account.AccountComponent.AppliedEntitlements.ContainsKey("DEVELOPER_ACCESS"))
        {
            log.Info($"{authInfo.UserName} attempted to get api access");
            return Results.Unauthorized();
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("AccountId", accountId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, account.UserName),
                new Claim(ClaimTypes.Role, "api_readonly"),
                new Claim(ClaimTypes.Role, "api_admin"),
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = TokenIssuer,
            Audience = TokenAudience,
            SigningCredentials = new SigningCredentials(Key(EvosConfiguration.GetApiKey()), SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var stringToken = tokenHandler.WriteToken(token);
        log.Info($"{authInfo.UserName} logged in for api access");
        return Results.Ok(stringToken);
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