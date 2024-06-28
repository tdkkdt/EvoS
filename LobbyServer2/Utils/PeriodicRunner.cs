using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace CentralServer.Utils;

public abstract class PeriodicRunner
{
    private static readonly ILog log = LogManager.GetLogger(typeof(PeriodicRunner));
    
    private readonly TimeSpan _period;
    private readonly CancellationToken _token;
    public bool IsEnabled { get; set; } = true;

    protected PeriodicRunner(CancellationToken token, TimeSpan period)
    {
        _period = period;
        _token = token;
    }

    public async Task Run()
    {
        string className = GetType().ToString();
        using PeriodicTimer timer = new PeriodicTimer(_period);
        await DoRun(className);
        while (
            !_token.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(_token))
        {
            await DoRun(className);
        }
    }

    private async Task DoRun(string className)
    {
        try
        {
            if (IsEnabled)
            {
                log.Debug($"Executing task {className}");
                await ExecuteAsync(_token);
            }
            else
            {
                log.Debug($"Skipping task {className}");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Failed to execute {className}", ex);
        }
    }

    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
}
