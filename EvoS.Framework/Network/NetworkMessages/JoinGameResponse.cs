using EvoS.Framework.Network.WebSocket;
using System;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(756)]
    public class JoinGameResponse : WebSocketResponseMessage
    {
        public LocalizationPayload LocalizedFailure;
    }
}
