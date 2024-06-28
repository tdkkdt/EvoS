using System;
using System.Threading;
using System.Threading.Tasks;
using CentralServer.Utils;
using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;
using Prometheus;

namespace CentralServer;

public class ServerStatisticsTask : PeriodicRunner
{
    private static readonly Gauge Audience = Metrics
        .CreateGauge(
            "evos_lobby_audience",
            "Number of people logged in within a specified window.",
            "window");
    
    public ServerStatisticsTask(CancellationToken token) : base(token, TimeSpan.FromMinutes(30))
    {
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        DateTime now = DateTime.UtcNow;
        AccountDao accountDao = DB.Get().AccountDao;
        long total = accountDao.GetUserCount();
        long mau = accountDao.GetUserCountWithLoginsSince(now - TimeSpan.FromDays(30));
        long wau = accountDao.GetUserCountWithLoginsSince(now - TimeSpan.FromDays(7));
        long dau = accountDao.GetUserCountWithLoginsSince(now - TimeSpan.FromDays(1));
        
        Audience.WithLabels("allTime").Set(total);
        Audience.WithLabels("month").Set(mau);
        Audience.WithLabels("week").Set(wau);
        Audience.WithLabels("day").Set(dau);
        
        return Task.CompletedTask;
    }
}