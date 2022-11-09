using EvoS.Framework.Network.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(367)]
    public class GroupLeaveResponse : WebSocketResponseMessage
    {
        public LocalizationPayload LocalizedFailure;
    }
}
