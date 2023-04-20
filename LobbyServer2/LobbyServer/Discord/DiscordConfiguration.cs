using WebSocketSharp;

namespace CentralServer.LobbyServer.Discord
{
    public class DiscordConfiguration
    {
        public bool Enabled = false;
        
        public DiscordChannel AdminChannel;
        public DiscordChannel GameLogChannel;
        public DiscordChannel LobbyChannel;
        public int LobbyChannelUpdatePeriodSeconds = 300;
        public bool LobbyChannelUpdateOnChangeOnly = true;
    }

    public class DiscordChannel
    {
        public string Webhook = "";
        public ulong? ThreadId = null;
    }

    public static class DiscordConfigExtensions
    {
        public static bool IsChannel(this DiscordChannel channel)
        {
            return channel != null
                   && channel.Webhook != null
                   && channel.Webhook.MaybeUri();
        }
    }
}
