using System;
using EvoS.Framework.Network;
using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.WebSocket;

namespace LobbyGameClientMessages;

[Serializable]
[EvosMessage(685, typeof(ClientPerformanceReport))]
public class ClientPerformanceReport : WebSocketMessage
{
    public LobbyGameClientPerformanceInfo PerformanceInfo;
}