namespace CentralServer.LobbyServer.Discord
{
    public class DiscordConfiguration
    {
        public bool Enabled = false;
        
        public string AdminChannelWebhook = "";
        
        public string ChannelWebhook = "";
        public ulong? ChannelThreadId = null;
        
        public bool DiscordLobbyEnabled = false;
        public string LobbyChannelWebhook = "";
        public ulong? LobbyChannelThreadId = null;
        public int LobbyChannelUpdatePeriodSeconds = 300;
        public bool LobbyChannelUpdateOnChangeOnly = true;
    }
}
