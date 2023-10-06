using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.WebSocket;
using System;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(715)]
    public class GameInfoUpdateRequest : WebSocketMessage
    {
        public LobbyGameInfo GameInfo;

        public LobbyTeamInfo TeamInfo;
    }
}
