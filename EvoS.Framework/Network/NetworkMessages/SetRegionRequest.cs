using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using EvoS.Framework.Constants.Enums;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(761)]
    public class SetRegionRequest : WebSocketMessage
    {
        public Region Region;
    }
}
