using System;
using System.Collections.Generic;
using EvoS.Framework.Network.Static;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(24)]
    public class CalculateFreelancerStatsResponse : WebSocketResponseMessage
    {
        public Dictionary<StatDisplaySettings.StatType, PercentileInfo> GlobalPercentiles;
        public Dictionary<int, PercentileInfo> FreelancerSpecificPercentiles;
        public LocalizationPayload LocalizedFailure;
    }
}