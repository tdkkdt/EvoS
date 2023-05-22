using System;

namespace EvoS.Framework.Network.Static
{
    [Serializable]
    [EvosMessage(606)]
    public class MatchDetailsComponent : ICloneable
    {
        public MatchDetailsComponent()
        {
        }
        
        public MatchDetailsComponent(PlayerGameSummary summary)
        {
            Deaths = summary.NumDeaths;
            Takedowns = summary.NumKills;
            DamageDealt = summary.TotalPlayerDamage;
            DamageTaken = summary.TotalPlayerDamageReceived;
            Healing = summary.TotalPlayerHealing;
            Contribution = summary.GetContribution();
            MatchResults = summary.MatchResults ?? new MatchResultsStats(); // null results in an exception on client side, default value results in placeholders
            GroupSize = -1;
            RankedTierLocTag = null;
            RankedPoints = 0;
        }

        public int Deaths { get; set; }

        public int Takedowns { get; set; }

        public int DamageDealt { get; set; }

        public int DamageTaken { get; set; }

        public int Healing { get; set; }

        public int Contribution { get; set; }

        public MatchResultsStats MatchResults { get; set; }

        public int GroupSize { get; set; }

        public string RankedTierLocTag { get; set; }

        public float RankedPoints { get; set; }

        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
