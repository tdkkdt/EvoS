using EvoS.Framework.Network.WebSocket;
using System;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(757)]
    public class JoinGameRequest : WebSocketMessage
    {
        public string GameServerProcessCode;

        public bool AsSpectator;
    }
}
