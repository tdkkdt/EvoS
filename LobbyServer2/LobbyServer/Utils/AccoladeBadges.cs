using System.Collections.Generic;
using System.Linq;
using EvoS.Framework.Misc;
using EvoS.Framework.Network.Static;

namespace CentralServer.LobbyServer.Utils
{
    // extracted from GameResultBadgeData asset
    public class AccoladeBadges
    {
        public static readonly List<GameResultBadgeData.ConsolidatedBadgeGroup> BadgeGroups = new List<GameResultBadgeData.ConsolidatedBadgeGroup>
        {
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Takedown Participation",
                BadgeGroupDescription = "Participate in enemy takedowns.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.General,
                BadgeIDs = new[] { 1, 2, 3 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Vision",
                BadgeGroupDescription = "Spot enemies over multiple turns without dying.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                BadgeIDs = new[] { 4, 5, 6 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Triple Threat",
                BadgeGroupDescription = "Have high Damage Per Turn, Support Per Turn, and Tanking Per Life.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.General,
                BadgeIDs = new[] { 7, 8, 9 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Freelancer Badge",
                BadgeGroupDescription = "Excel in your Freelancer-specific stats.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.General,
                BadgeIDs = new[] { 10, 11, 12 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Damage Per Turn",
                BadgeGroupDescription = "Deal high Damage Per Turn",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                BadgeIDs = new[] { 14, 15, 16 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Damage Efficiency",
                BadgeGroupDescription = "Deal damage without losing effectiveness to Cover or Overkill.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                BadgeIDs = new[] { 17, 18, 19 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Damage Done Per Life",
                BadgeGroupDescription = "Deal a high amount of damage while dying as little as possible.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                BadgeIDs = new[] { 21, 22, 23 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Tanking",
                BadgeGroupDescription = "Take damage while dying as little as possible.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                BadgeIDs = new[] { 24, 25, 26 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Damage Dodged",
                BadgeGroupDescription = "Dodge a high amount of damage.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.General,
                BadgeIDs = new[] { 27, 28, 29 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Crowd Control",
                BadgeGroupDescription = "Deny movement to enemies with Slow, Root, and Knockback abilities.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                BadgeIDs = new[] { 30, 31, 32 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Support Per Turn",
                BadgeGroupDescription = "Heal and Shield your allies from damage.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                BadgeIDs = new[] { 34, 35, 36 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Team Damage Swing",
                BadgeGroupDescription = "Boost your team's damage with Might and ruin the enemy team's damage with Weaken.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                BadgeIDs = new[] { 37, 38, 39 }
            },
            new GameResultBadgeData.ConsolidatedBadgeGroup
            {
                BadgeGroupDisplayName = "Team Energizer",
                BadgeGroupDescription = "Grant your team energy with Energized or other effects.",
                DisplayCategory = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                BadgeIDs = new[] { 40, 41, 42 }
            }
        };
        
        public static readonly Dictionary<int, GameBalanceVars.GameResultBadge> GameResultBadges =
            new List<GameBalanceVars.GameResultBadge>
            {
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Takedown Participation",
                    BadgeDescription = "Participate in at least 3 enemy takedowns.",
                    BadgeGroupRequirementDescription = "At least 3 enemy takedowns.",
                    BadgeIconString = "BadgesStats/TakedownParticipation_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 1,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.TotalAssists
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType
                                .GreaterThanOrEqual,
                            ValueToCompare = 3.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.KillParticipation
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.None,
                    GlobalPercentileToObtain = 1,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Takedown Participation",
                    BadgeDescription = "Participate in at least 4 enemy takedowns.",
                    BadgeGroupRequirementDescription = "At least 4 enemy takedowns.",
                    BadgeIconString = "BadgesStats/TakedownParticipation_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 2,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.TotalAssists
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 4.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.KillParticipation
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.None,
                    GlobalPercentileToObtain = 0,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Takedown Participation",
                    BadgeDescription = "Participate in at least 5 enemy takedowns.",
                    BadgeGroupRequirementDescription = "At least 5 enemy takedowns.",
                    BadgeIconString = "BadgesStats/TakedownParticipation_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 3,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.TotalAssists
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType
                                .GreaterThanOrEqual,
                            ValueToCompare = 5.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.KillParticipation
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.None,
                    GlobalPercentileToObtain = 0,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Vision",
                    BadgeDescription =
                        "Spot at least 3 enemies per life, and spot more enemies per life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Vision_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 4,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.EnemiesSightedPerLife
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 3.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.EnemiesSightedPerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TotalDeaths,
                        StatDisplaySettings.StatType.EnemiesSightedPerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Vision",
                    BadgeDescription =
                        "Spot at least 4 enemies per life, and spot more enemies per life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Vision_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 5,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.EnemiesSightedPerLife
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType
                                .GreaterThanOrEqual,
                            ValueToCompare = 4.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.EnemiesSightedPerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TotalDeaths,
                        StatDisplaySettings.StatType.EnemiesSightedPerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Vision",
                    BadgeDescription =
                        "Spot at least 5 enemies per life, and spot more enemies per life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Vision_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 6,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.EnemiesSightedPerLife
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 5.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.EnemiesSightedPerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TotalDeaths,
                        StatDisplaySettings.StatType.EnemiesSightedPerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Triple Threat",
                    BadgeDescription =
                        "Deal at least 10 Damage Per Turn, 10 Support Per Turn, and tank at least 100 damage per life.",
                    BadgeGroupRequirementDescription =
                        "At least 10 Damage Per Turn, 10 Support Per Turn, and 100 tanking per life.",
                    BadgeIconString = "BadgesStats/TripleThreat_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 7,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.DamagePerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 10.0f
                        },
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.SupportPerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 10.0f
                        },
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.TankingPerLife
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 100.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamagePerTurn,
                        StatDisplaySettings.StatType.SupportPerTurn,
                        StatDisplaySettings.StatType.TankingPerLife,
                        StatDisplaySettings.StatType.DamageTakenPerLife,
                        StatDisplaySettings.StatType.EffectiveHealAndAbsorb,
                        StatDisplaySettings.StatType.IncomingDamageDodgeByEvade,
                        StatDisplaySettings.StatType.IncomingDamageReducedByCover
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.None,
                    GlobalPercentileToObtain = 0,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Triple Threat",
                    BadgeDescription =
                        "Deal at least 15 Damage Per Turn, 15 Support Per Turn, and tank at least 150 damage per life.",
                    BadgeGroupRequirementDescription =
                        "At least 15 Damage Per Turn, 15 Support Per Turn, and 150 tanking per life.",
                    BadgeIconString = "BadgesStats/TripleThreat_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 8,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.DamagePerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 15.0f
                        },
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.SupportPerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 15.0f
                        },
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.TankingPerLife
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 150.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.SupportPerTurn,
                        StatDisplaySettings.StatType.TankingPerLife,
                        StatDisplaySettings.StatType.DamagePerTurn,
                        StatDisplaySettings.StatType.EffectiveHealAndAbsorb,
                        StatDisplaySettings.StatType.DamageTakenPerLife,
                        StatDisplaySettings.StatType.IncomingDamageDodgeByEvade,
                        StatDisplaySettings.StatType.IncomingDamageReducedByCover
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.None,
                    GlobalPercentileToObtain = 1,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Triple Threat",
                    BadgeDescription =
                        "Deal at least 20 Damage Per Turn, 20 Support Per Turn, and tank at least 200 damage per life.",
                    BadgeGroupRequirementDescription =
                        "At least 20 Damage Per Turn, 20 Support Per Turn, and 200 tanking per life.",
                    BadgeIconString = "BadgesStats/TripleThreat_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 9,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.DamagePerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 20.0f
                        },
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.SupportPerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 20.0f
                        },
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.TankingPerLife
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 200.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.SupportPerTurn,
                        StatDisplaySettings.StatType.TankingPerLife,
                        StatDisplaySettings.StatType.DamagePerTurn,
                        StatDisplaySettings.StatType.EffectiveHealAndAbsorb,
                        StatDisplaySettings.StatType.DamageTakenPerLife,
                        StatDisplaySettings.StatType.IncomingDamageDodgeByEvade,
                        StatDisplaySettings.StatType.IncomingDamageReducedByCover
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.None,
                    GlobalPercentileToObtain = 0,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Freelancer Badge",
                    BadgeDescription =
                        "Beat [PercentileToObtain]% of [FreelancerName]'s players in your [FreelancerName]-specific stats.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/FreelancerBadge_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 10,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>(),
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Freelancer,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = true
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Freelancer Badge",
                    BadgeDescription =
                        "Beat [PercentileToObtain]% of [FreelancerName]'s players in your [FreelancerName]-specific stats.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/FreelancerBadge_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 11,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>(),
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Freelancer,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = true
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Freelancer Badge",
                    BadgeDescription =
                        "Beat [PercentileToObtain]% of [FreelancerName]'s players in your [FreelancerName]-specific stats.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/FreelancerBadge_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 12,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.TotalAssists,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>(),
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Freelancer,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = true
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Best In Match: Damage Per Turn",
                    BadgeDescription = "Deal the highest Damage Per Turn in a game.",
                    BadgeGroupRequirementDescription = "",
                    BadgeIconString = "BadgesStats/HighestDamagePerTurnInGame_01_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 13,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamagePerTurn,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamagePerTurn
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Game,
                    GlobalPercentileToObtain = 0,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Per Turn",
                    BadgeDescription = "Deal more Damage Per Turn than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamagePerTurn_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 14,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamagePerTurn,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamagePerTurn
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Per Turn",
                    BadgeDescription = "Deal more Damage Per Turn than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamagePerTurn_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 15,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamagePerTurn,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamagePerTurn
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Per Turn",
                    BadgeDescription = "Deal more Damage Per Turn than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamagePerTurn_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 16,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamagePerTurn,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamagePerTurn
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Efficiency",
                    BadgeDescription =
                        "Have better Damage Efficiency than [PercentileToObtain]% of all players.  Damage Efficiency is based on your total damage done, minus your Overkill Damage and your Damage Lost To Cover.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageEfficiency_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 17,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.DamagePerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 15.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageEfficiency,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageEfficiency
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Efficiency",
                    BadgeDescription =
                        "Have better Damage Efficiency than [PercentileToObtain]% of all players.  Damage Efficiency is based on your total damage done, minus your Overkill Damage and your Damage Lost To Cover.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageEfficiency_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 18,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.DamagePerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType
                                .GreaterThanOrEqual,
                            ValueToCompare = 20.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageEfficiency,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageEfficiency
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Efficiency",
                    BadgeDescription =
                        "Have better Damage Efficiency than [PercentileToObtain]% of all players.  Damage Efficiency is based on your total damage done, minus your Overkill Damage and your Damage Lost To Cover.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageEfficiency_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 19,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.DamagePerTurn
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThanOrEqual,
                            ValueToCompare = 25.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageEfficiency,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageEfficiency
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Best In Match: Tanking",
                    BadgeDescription = "Have the highest Damage Taken per Life in the game.",
                    BadgeGroupRequirementDescription = "",
                    BadgeIconString = "BadgesStats/HighestTankingInGame_01_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 20,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageTakenPerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageTakenPerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Game,
                    GlobalPercentileToObtain = 0,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Done Per Life",
                    BadgeDescription = "Have higher Damage Done Per Life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageDonePerLife_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 21,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageDonePerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageDonePerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Done Per Life",
                    BadgeDescription = "Have higher Damage Done Per Life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageDonePerLife_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 22,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageDonePerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageDonePerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Done Per Life",
                    BadgeDescription = "Have higher Damage Done Per Life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageDonePerLife_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 23,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Firepower,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageDonePerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageDonePerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Tanking",
                    BadgeDescription = "Have higher Damage Taken per Life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Tanking_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 24,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageTakenPerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.IncomingDamageReducedByCover
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Tanking",
                    BadgeDescription = "Have higher Damage Taken per Life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Tanking_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 25,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageTakenPerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageTakenPerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Tanking",
                    BadgeDescription = "Have higher Damage Taken per Life than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Tanking_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 26,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.DamageTakenPerLife,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.DamageTakenPerLife
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Dodged",
                    BadgeDescription = "Dodge more damage than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageDodged_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 27,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.IncomingDamageDodgeByEvade,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.IncomingDamageDodgeByEvade
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Dodged",
                    BadgeDescription = "Dodge more damage than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageDodged_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 28,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.IncomingDamageDodgeByEvade,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.IncomingDamageDodgeByEvade
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Damage Dodged",
                    BadgeDescription = "Dodge more damage than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/DamageDodged_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 29,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.General,
                    BadgePointCalcType = StatDisplaySettings.StatType.IncomingDamageDodgeByEvade,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.IncomingDamageDodgeByEvade
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Crowd Control",
                    BadgeDescription = "Deny more movement to enemies than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/CrowdControl_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 30,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.MovementDenied,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.MovementDenied
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Crowd Control",
                    BadgeDescription = "Deny more movement to enemies than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/CrowdControl_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 31,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.MovementDenied,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.MovementDenied
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Crowd Control",
                    BadgeDescription = "Deny more movement to enemies than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/CrowdControl_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 32,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Frontliner,
                    BadgePointCalcType = StatDisplaySettings.StatType.MovementDenied,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.MovementDenied
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Best In Match: Team Damage Mitigated",
                    BadgeDescription = "Mitigate the highest percentage of team damage in the game.",
                    BadgeGroupRequirementDescription = "",
                    BadgeIconString = "BadgesStats/HighestTeamDamageMitigated_01_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 33,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.TeamMitigation,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.SupportPerTurn,
                        StatDisplaySettings.StatType.EffectiveHealAndAbsorb
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Game,
                    GlobalPercentileToObtain = 1,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Healed/Shielded Per Turn",
                    BadgeDescription =
                        "Heal and Shield more damage per turn than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/HealedShieldedPerTurn_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 34,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.EffectiveHealAndAbsorb
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThan,
                            ValueToCompare = 5.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.EffectiveHealAndAbsorb,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.SupportPerTurn,
                        StatDisplaySettings.StatType.EffectiveHealAndAbsorb
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Healed/Shielded Per Turn",
                    BadgeDescription =
                        "Heal and Shield more damage per turn than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/HealedShieldedPerTurn_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 35,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.EffectiveHealAndAbsorb
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThan,
                            ValueToCompare = 8.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.EffectiveHealAndAbsorb,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.SupportPerTurn,
                        StatDisplaySettings.StatType.EffectiveHealAndAbsorb
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Healed/Shielded Per Turn",
                    BadgeDescription =
                        "Heal and Shield more damage per turn than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/HealedShieldedPerTurn_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 36,
                    MinimumConditions = new[]
                    {
                        new GameBalanceVars.GameResultBadge.BadgeCondition
                        {
                            StatsToSum = new[]
                            {
                                StatDisplaySettings.StatType.EffectiveHealAndAbsorb
                            },
                            FunctionType = GameBalanceVars.GameResultBadge.BadgeCondition.BadgeFunctionType.GreaterThan,
                            ValueToCompare = 12.0f
                        }
                    },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.EffectiveHealAndAbsorb,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.SupportPerTurn,
                        StatDisplaySettings.StatType.EffectiveHealAndAbsorb
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Team Damage Swing",
                    BadgeDescription =
                        "Boost your team's damage with Might and ruin your enemy team's damage with Weaken more than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/StatusEffects_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 37,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.TeamDamageAdjustedByMe,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TeamDamageAdjustedByMe
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Team Damage Swing",
                    BadgeDescription =
                        "Boost your team's damage with Might and ruin your enemy team's damage with Weaken more than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/StatusEffects_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 38,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.TeamDamageAdjustedByMe,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TeamDamageAdjustedByMe
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Team Damage Swing",
                    BadgeDescription =
                        "Boost your team's damage with Might and ruin your enemy team's damage with Weaken more than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/StatusEffects_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 39,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.TeamDamageAdjustedByMe,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TeamDamageAdjustedByMe
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Energized",
                    BadgeDescription =
                        "Grant your team more energy with Energized or other effects than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Energized_01_bronze",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 40,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Bronze,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.TeamExtraEnergyByEnergizedFromMe,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TeamExtraEnergyByEnergizedFromMe
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 50,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Energized",
                    BadgeDescription =
                        "Grant your team more energy with Energized or other effects than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Energized_02_silver",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 41,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Silver,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.TeamExtraEnergyByEnergizedFromMe,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TeamExtraEnergyByEnergizedFromMe
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 65,
                    UsesFreelancerStats = false
                },
                new GameBalanceVars.GameResultBadge
                {
                    DisplayName = "Energized",
                    BadgeDescription =
                        "Grant your team more energy with Energized or other effects than [PercentileToObtain]% of all players.",
                    BadgeGroupRequirementDescription = "Better than [PercentileToObtain]% of all players.",
                    BadgeIconString = "BadgesStats/Energized_03_gold",
                    DisplayEvenIfConsolidated = false,
                    UniqueBadgeID = 42,
                    MinimumConditions = new GameBalanceVars.GameResultBadge.BadgeCondition[] { },
                    Quality = GameBalanceVars.GameResultBadge.BadgeQuality.Gold,
                    Role = GameBalanceVars.GameResultBadge.BadgeRole.Support,
                    BadgePointCalcType = StatDisplaySettings.StatType.TeamExtraEnergyByEnergizedFromMe,
                    StatsToHighlight = new List<StatDisplaySettings.StatType>
                    {
                        StatDisplaySettings.StatType.TeamExtraEnergyByEnergizedFromMe
                    },
                    ComparisonGroup = GameBalanceVars.GameResultBadge.ComparisonType.Global,
                    GlobalPercentileToObtain = 80,
                    UsesFreelancerStats = false
                }
            }
                .ToDictionary(badge => badge.GetID());
    }
}