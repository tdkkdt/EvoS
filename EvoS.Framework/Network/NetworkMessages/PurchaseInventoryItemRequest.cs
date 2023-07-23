using System;
using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(280)]
    public class PurchaseInventoryItemRequest : WebSocketMessage
    {
        public int InventoryItemID;
        public CurrencyType CurrencyType;
    }
}
