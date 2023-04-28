using System;
using EvoS.Framework.Network;

namespace EvoS.Framework.Misc
{
    [EvosMessage(421)]
    [Serializable]
    public class QuestItemReward
    {
        public int ItemTemplateId;
        public int Amount;
    }
}