using EvoS.Framework.Network.WebSocket;
using System;
using System.Collections.Generic;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(450)]
    public class BalancedTeamResponse : WebSocketResponseMessage
    {
        [EvosMessage(451)]
        public List<BalanceTeamSlot> Slots;

        public LocalizationPayload LocalizedFailure;
    }
}
