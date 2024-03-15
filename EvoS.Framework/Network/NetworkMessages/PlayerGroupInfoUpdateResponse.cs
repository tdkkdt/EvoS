using System;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(730)]
    public class PlayerGroupInfoUpdateResponse : WebSocketResponseMessage
    {
        public LocalizationPayload LocalizedFailure;
    }
}
