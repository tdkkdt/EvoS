using EvoS.Framework.Constants.Enums;
using System;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(453)]
    public struct BalanceTeamSlot
    {
        public Team Team;

        public int PlayerId;

        public long AccountId;

        public CharacterType SelectedCharacter;

        public BotDifficulty BotDifficulty;
    }
}
