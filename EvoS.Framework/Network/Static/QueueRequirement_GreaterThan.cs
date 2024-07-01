using System;

namespace EvoS.Framework.Network.Static
{
    [Serializable]
    [EvosMessage(793)]
    public class QueueRequirement_GreaterThan : QueueRequirement
    {
        public int MinValue { get; set; }

        public override bool AnyGroupMember => m_anyGroupMember;

        public override RequirementType Requirement => m_requirementType;


        private RequirementType m_requirementType;

        private bool m_anyGroupMember;

        public QueueRequirement_GreaterThan(RequirementType mRequirementType, bool mAnyGroupMember)
        {
            m_requirementType = mRequirementType;
            m_anyGroupMember = mAnyGroupMember;
        }
    }
}
