using EvoS.Framework.Network.WebSocket;
using System;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(406)]
    public class UpdateRemoteCharacterRequest : WebSocketMessage
    {
        public CharacterType[] Characters;

        public int[] RemoteSlotIndexes;
    }
}
