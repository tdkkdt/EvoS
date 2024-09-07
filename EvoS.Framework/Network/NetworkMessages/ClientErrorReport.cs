using System;
using EvoS.Framework.Network;
using EvoS.Framework.Network.WebSocket;

namespace LobbyGameClientMessages;

[Serializable]
[EvosMessage(690, typeof(ClientErrorReport))]
public class ClientErrorReport : WebSocketMessage
{
    public string LogString;
    public string StackTrace;
    public uint StackTraceHash;
    public float Time;

    public int CalcBytes()
    {
        return LogString.Length + StackTrace.Length + 4 + 4;
    }
}