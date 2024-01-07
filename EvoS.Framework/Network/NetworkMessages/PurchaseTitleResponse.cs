using System;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages;

[Serializable]
public class PurchaseTitleResponse : WebSocketResponseMessage
{
    public PurchaseResult Result;
    public CurrencyType CurrencyType;
    public int TitleId;
}
    