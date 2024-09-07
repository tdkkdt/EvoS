using System;

namespace EvoS.Framework.Network.Static;

[Serializable]
[EvosMessage(686, typeof(LobbyGameClientPerformanceInfo))]
public class LobbyGameClientPerformanceInfo
{
    public float AvgFPS;
    public float AvgLatency;
    public float AvgCpuUsage;
    public float AvgMemoryUsage;
    public float CurrentFPS;
    public float CurrentLatency;
    public float CurrentCpuUsage;
    public float CurrentMemoryUsage;
    
    public override string ToString()
    {
        return
            $"{nameof(AvgFPS)}: {AvgFPS}, "
            + $"{nameof(AvgLatency)}: {AvgLatency}, "
            + $"{nameof(AvgCpuUsage)}: {AvgCpuUsage}, "
            + $"{nameof(AvgMemoryUsage)}: {AvgMemoryUsage}, "
            + $"{nameof(CurrentFPS)}: {CurrentFPS}, "
            + $"{nameof(CurrentLatency)}: {CurrentLatency}, "
            + $"{nameof(CurrentCpuUsage)}: {CurrentCpuUsage}, "
            + $"{nameof(CurrentMemoryUsage)}: {CurrentMemoryUsage}";
    }
}