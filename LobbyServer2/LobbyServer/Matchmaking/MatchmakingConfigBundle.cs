using System.Collections.Generic;

namespace CentralServer.LobbyServer.Matchmaking;

public class MatchmakingConfigBundle
{
    public const string DEFAULT = "default";
    
    public Dictionary<string, MatchmakingConfiguration> subTypes = new() {
        [DEFAULT] = new()
    };
    
    public MatchmakingConfiguration Default => subTypes[DEFAULT];
}