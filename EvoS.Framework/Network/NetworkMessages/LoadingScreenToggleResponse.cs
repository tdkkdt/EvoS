using System;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(397)]
    public class LoadingScreenToggleResponse : WebSocketResponseMessage
    {
        public int LoadingScreenId;
        public bool CurrentState;
    }
}
