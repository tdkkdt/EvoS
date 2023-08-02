using EvoS.Framework.Network.WebSocket;
using System;
using System.Collections.Generic;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(454)]
    public class BalancedTeamRequest : WebSocketMessage
    {
        [EvosMessage(451)]
        public List<BalanceTeamSlot> Slots;
    }
}
