using System;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(398)]
    public class LoadingScreenToggleRequest : WebSocketMessage
    {
        public int LoadingScreenId;
        public bool NewState;
    }
}
