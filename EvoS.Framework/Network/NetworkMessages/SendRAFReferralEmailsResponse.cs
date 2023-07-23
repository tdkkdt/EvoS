using System;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(257)]
    public class SendRAFReferralEmailsResponse : WebSocketResponseMessage
    {
    }
}
