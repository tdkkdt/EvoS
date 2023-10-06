using EvoS.Framework.Network.WebSocket;
using System;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(405)]
    public class UpdateRemoteCharacterResponse : WebSocketResponseMessage
    {
    }
}
