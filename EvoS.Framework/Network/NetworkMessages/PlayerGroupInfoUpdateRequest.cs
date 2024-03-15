using System;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(731)]
    public class PlayerGroupInfoUpdateRequest : WebSocketMessage
    {
        public GameType GameType;
    }
}
