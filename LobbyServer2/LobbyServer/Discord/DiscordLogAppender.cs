using System.Collections.Generic;
using log4net.Appender;
using log4net.Core;

namespace CentralServer.LobbyServer.Discord
{
    public class DiscordLogAppender: AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            _ = DiscordManager.Get().SendLogEvent(loggingEvent.Level, RenderLoggingEvent(loggingEvent));
        }
    }
}