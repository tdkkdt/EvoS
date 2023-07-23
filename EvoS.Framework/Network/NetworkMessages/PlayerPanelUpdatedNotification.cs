using System;
using EvoS.Framework.Network.WebSocket;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(281)]
    public class PlayerPanelUpdatedNotification : WebSocketMessage
    {
        public int originalSelectedTitleID { get; set; }
        public int originalSelectedForegroundBannerID { get; set; }
        public int originalSelectedBackgroundBannerID { get; set; }
        public int originalSelectedRibbonID { get; set; }
    }
}