using EvoS.Framework.Network.WebSocket;
using System;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(408)]
    public class SetDevTagResponse : WebSocketResponseMessage
    {
    }
}
