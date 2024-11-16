using Prometheus;

namespace CentralServer.Utils;

public static class MetricUtils
{
    public static void Zero(this Collector<Gauge.Child> m)
    {
        foreach (var labelValues in m.GetAllLabelValues())
        {
            m.WithLabels(labelValues).Set(0);
        }
    }
}