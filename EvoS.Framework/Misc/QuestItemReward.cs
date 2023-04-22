using EvoS.Framework.Network;

namespace EvoS.Framework.Misc
{
    [EvosMessage(421)]
    public class QuestItemReward
    {
        public int ItemTemplateId;
        public int Amount;
    }
}