using EvoS.Framework.Network.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(279)]
    public class PurchaseInventoryItemResponse : WebSocketResponseMessage
    {
        public PurchaseResult Result;
        public CurrencyType CurrencyType;
        public int InventoryItemID;
    }
}
