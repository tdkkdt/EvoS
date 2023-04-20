using System.Threading.Tasks;
using Discord;
using log4net;

namespace CentralServer.LobbyServer.Discord
{
    public static class DiscordUtils
    {
        public static Task Log(ILog log, LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Debug:
                case LogSeverity.Verbose:
                    log.Debug(msg.Message);
                    break;
                case LogSeverity.Info:
                    log.Info(msg.Message);
                    break;
                case LogSeverity.Warning:
                    log.Warn(msg.Message);
                    break;
                case LogSeverity.Critical:
                    log.Error(msg.Message, msg.Exception);
                    break;
            }

            return Task.CompletedTask;
        }
        
    }
}