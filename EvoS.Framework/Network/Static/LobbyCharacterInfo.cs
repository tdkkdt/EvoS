using System;
using System.Collections.Generic;
using System.Linq;

namespace EvoS.Framework.Network.Static
{
    [Serializable]
    [EvosMessage(552)]
    public class LobbyCharacterInfo
    {
        public LobbyCharacterInfo Clone()
        {
            return (LobbyCharacterInfo)base.MemberwiseClone();
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
        
        private static readonly Dictionary<CharacterType, CharacterAbilityConfigOverride> s_overrides = new();

        static LobbyCharacterInfo()
        {
            BanMod(CharacterType.Manta, 4, 1); // Phaedra's "AfterShock"
            BanMod(CharacterType.Claymore, 1, 3); // Titus' "Single Minded"
            BanMod(CharacterType.Tracker, 4, 1); // Grey's "Overcharged coils"
            BanMod(CharacterType.Spark, 4, 5); // Quark's "Piercing Light"
            BanMod(CharacterType.Blaster, 0, 5); // Elle's "Long Barrel"
        }

        private static void BanMod(CharacterType characterType, int abilityIndex, int modAbilityScopeId)
        {
            if (!s_overrides.TryGetValue(characterType, out CharacterAbilityConfigOverride confOverride))
            {
                confOverride = new CharacterAbilityConfigOverride(characterType);
                s_overrides.Add(characterType, confOverride);
            }
            confOverride.AbilityConfigs[abilityIndex] = new AbilityConfigOverride(characterType, abilityIndex)
            {
                AbilityModConfigs = new Dictionary<int, AbilityModConfigOverride>
                {
                    {
                        modAbilityScopeId,
                        new AbilityModConfigOverride
                        {
                            AbilityIndex = abilityIndex,
                            AbilityModIndex = modAbilityScopeId,
                            Allowed = false,
                            CharacterType = characterType
                        }
                    }
                }
            };
        }

        public static LobbyCharacterInfo Of(PersistedCharacterData data)
        {
            return Of(data, data.CharacterComponent);
        }

        public static LobbyCharacterInfo Of(PersistedCharacterData data, CharacterComponent cc)
        {
            return new LobbyCharacterInfo
            {
                CharacterType = data.CharacterType,
                CharacterSkin = cc.LastSkin,
                CharacterCards = cc.LastCards,
                CharacterMods = RemoveDisabledMods(cc.LastMods, data.CharacterType),
                CharacterAbilityVfxSwaps = cc.LastAbilityVfxSwaps,
                CharacterTaunts = cc.Taunts,
                CharacterLoadouts = cc.CharacterLoadouts,
                CharacterMatches = data.ExperienceComponent.Matches,
                CharacterLevel = data.ExperienceComponent.Level
            };
        }

        public static CharacterModInfo RemoveDisabledMods(CharacterModInfo lastMods, CharacterType characterType)
        {
            var characterAbilityConfigOverrides = GetChacterAbilityConfigOverrides();
            var abilityConfigs = characterAbilityConfigOverrides
                .Where(c => c.Key == characterType)
                .SelectMany(c => c.Value.AbilityConfigs)
                .OfType<AbilityConfigOverride>();

            foreach (var abilityConfig in abilityConfigs)
            {
                foreach (var abilityModConfig in abilityConfig.AbilityModConfigs)
                {
                    switch (abilityModConfig.Value.AbilityIndex)
                    {
                        case 0 when abilityModConfig.Value.AbilityModIndex == lastMods.ModForAbility0:
                            lastMods.ModForAbility0 = 0;
                            break;
                        case 1 when abilityModConfig.Value.AbilityModIndex == lastMods.ModForAbility1:
                            lastMods.ModForAbility1 = 0;
                            break;
                        case 2 when abilityModConfig.Value.AbilityModIndex == lastMods.ModForAbility2:
                            lastMods.ModForAbility2 = 0;
                            break;
                        case 3 when abilityModConfig.Value.AbilityModIndex == lastMods.ModForAbility3:
                            lastMods.ModForAbility3 = 0;
                            break;
                        case 4 when abilityModConfig.Value.AbilityModIndex == lastMods.ModForAbility4:
                            lastMods.ModForAbility4 = 0;
                            break;
                    }
                }
            }
            return lastMods;
        }

        public static Dictionary<CharacterType, CharacterAbilityConfigOverride> GetChacterAbilityConfigOverrides()
        {
            return s_overrides;
        }
    }
}
