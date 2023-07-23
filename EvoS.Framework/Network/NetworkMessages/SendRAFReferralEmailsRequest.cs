using System;
using System.Collections.Generic;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(258)]
    public class SendRAFReferralEmailsRequest : WebSocketMessage
    {
        public List<string> Emails;
    }
}
