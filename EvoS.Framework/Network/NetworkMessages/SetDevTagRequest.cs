using EvoS.Framework.Network.WebSocket;
using System;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(409)]
    public class SetDevTagRequest : WebSocketMessage
    {
        public bool active;
    }
}
