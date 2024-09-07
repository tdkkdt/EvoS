using System;
using EvoS.Framework.Network;
using EvoS.Framework.Network.WebSocket;

namespace LobbyGameClientMessages;

[Serializable]
[EvosMessage(691, typeof(ErrorReportSummaryRequest))]
public class ErrorReportSummaryRequest : WebSocketMessage
{
    public uint CrashReportHash;
}