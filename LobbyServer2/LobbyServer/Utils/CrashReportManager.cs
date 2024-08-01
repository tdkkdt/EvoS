using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EvoS.Framework.Network.NetworkMessages;

namespace CentralServer.LobbyServer.Utils;

public static class CrashReportManager
{
    private static readonly Dictionary<long, Guid> PendingCrashReportGuids = new();

    public static Action<long, Stream> OnCrashReport = delegate {};
    public static Action<long, ClientStatusReport> OnStatusReport = delegate {};

    public static Guid Add(long accountId)
    {
        Guid guid = Guid.NewGuid();
        PendingCrashReportGuids[accountId] = guid;
        return guid;
    }

    public static long Pop(Guid guid)
    {
        long accountId = PendingCrashReportGuids.FirstOrDefault(e => e.Value == guid).Key;
        PendingCrashReportGuids.Remove(accountId);
        return accountId;
    }

    public static void ProcessArchive(long accountId, Stream archive)
    {
        OnCrashReport(accountId, archive);
    }

    public static void ProcessClientStatusReport(long accountId, ClientStatusReport report)
    {
        OnStatusReport(accountId, report);
    }
}