using CentralServer.ApiServer;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace CentralServer;

public class AdminServer
{
    private static readonly ILog log = LogManager.GetLogger(typeof(AdminServer));
    public WebApplication Init()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Logging.ClearProviders();
        var app = builder.Build();
        
        app.MapGet("/api/status", CommonController.GetStatus);
        _ = app.RunAsync("http://localhost:3000");
        
        log.Info("Started admin server localhost:3000");
        return app;
    }
}