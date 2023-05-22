using System.Threading.Tasks;
using Discord;
using log4net;
using log4net.Core;

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
                    log.Debug(msg.Message, msg.Exception);
                    break;
                case LogSeverity.Info:
                    log.Info(msg.Message, msg.Exception);
                    break;
                case LogSeverity.Warning:
                    log.Warn(msg.Message, msg.Exception);
                    break;
                case LogSeverity.Critical:
                    log.Error(msg.Message, msg.Exception);
                    break;
            }

            return Task.CompletedTask;
        }

        public static Color GetLogColor(Level level)
        {
            if (level >= Level.Error) return Color.DarkRed;
            if (level >= Level.Warn) return Color.Orange;
            if (level >= Level.Info) return Color.Green;
            return Color.Default;
        }
        
    }
}