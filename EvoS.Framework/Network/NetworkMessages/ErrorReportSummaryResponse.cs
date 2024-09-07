using System;
using EvoS.Framework.Network;
using EvoS.Framework.Network.WebSocket;

namespace LobbyGameClientMessages;

[Serializable]
[EvosMessage(689, typeof(ErrorReportSummaryResponse))]
public class ErrorReportSummaryResponse : WebSocketResponseMessage
{
    public ClientErrorReport ClientErrorReport;
}