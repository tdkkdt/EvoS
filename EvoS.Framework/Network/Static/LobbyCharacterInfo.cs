using System;
using System.Collections.Generic;

namespace EvoS.Framework.Network.Static
{
    [Serializable]
    [EvosMessage(552)]
    public class LobbyCharacterInfo
    {
        public LobbyCharacterInfo Clone()
        {
            return (LobbyCharacterInfo) base.MemberwiseClone();
        }

        public CharacterType CharacterType;

        public CharacterVisualInfo CharacterSkin = default(CharacterVisualInfo);

        public CharacterCardInfo CharacterCards = default(CharacterCardInfo);

        public CharacterModInfo CharacterMods = default(CharacterModInfo);

        public CharacterAbilityVfxSwapInfo CharacterAbilityVfxSwaps = default(CharacterAbilityVfxSwapInfo);

        [EvosMessage(528)] public List<PlayerTauntData> CharacterTaunts = new List<PlayerTauntData>();

        [EvosMessage(544)] public List<CharacterLoadout> CharacterLoadouts = new List<CharacterLoadout>();

        public int CharacterMatches;

        public int CharacterLevel;

        public static LobbyCharacterInfo Of(PersistedCharacterData data)
        {
            CharacterComponent cc = data.CharacterComponent;
            return new LobbyCharacterInfo
            {
                CharacterType = data.CharacterType,
                CharacterSkin = cc.LastSkin,
                CharacterCards = cc.LastCards,
                CharacterMods = cc.LastMods,
                CharacterAbilityVfxSwaps = cc.LastAbilityVfxSwaps,
                CharacterTaunts = cc.Taunts,
                CharacterLoadouts = cc.CharacterLoadouts,
                CharacterMatches = data.ExperienceComponent.Matches,
                CharacterLevel = data.ExperienceComponent.Level
            };
        }
    }
}
