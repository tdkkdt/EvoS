using System.Collections.Generic;
using System.Linq;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Network.Static;

namespace CentralServer.LobbyServer.Utils
{
    public static class AccoladesUtils
    {

        public static List<BadgeAndParticipantInfo> ProcessGameSummary(ServerGameSummaryNotification request)
        {
            List<BadgeAndParticipantInfo> result = new List<BadgeAndParticipantInfo>();
            if (request.GameSummary.GameResult != GameResult.TeamAWon
                && request.GameSummary.GameResult != GameResult.TeamBWon)
            {
                return result;
            }
            PlayerGameSummary highestHealingPlayer = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetTotalHealingFromAbility() + p.TotalPlayerAbsorb)
                .FirstOrDefault();
            PlayerGameSummary highestDamagePlayer = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.TotalPlayerDamage)
                .FirstOrDefault();
            PlayerGameSummary highestDamageReceivedPlayer = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.TotalPlayerDamageReceived)
                .FirstOrDefault();
            PlayerGameSummary highestDamagePerTurn = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetDamageDealtPerTurn())
                .FirstOrDefault();
            PlayerGameSummary highestDamageTakenPerLife = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetDamageTakenPerLife())
                .FirstOrDefault();
            PlayerGameSummary highestEnemiesSightedPerTurn = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.EnemiesSightedPerTurn)
                .FirstOrDefault();
            PlayerGameSummary highestMitigated = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetTeamMitigation())
                .FirstOrDefault();
            PlayerGameSummary highestDamageEfficiency = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.DamageEfficiency)
                .FirstOrDefault();
            PlayerGameSummary highestDamageDonePerLife = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetDamageDonePerLife())
                .FirstOrDefault();
            PlayerGameSummary highestDodge = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.DamageAvoidedByEvades)
                .FirstOrDefault();
            PlayerGameSummary highestCrowdControl = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.MovementDeniedByMe)
                .FirstOrDefault();
            PlayerGameSummary highestBoostTeamDamage = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.MyOutgoingExtraDamageFromEmpowered)
                .FirstOrDefault();
            PlayerGameSummary highestBoostTeamEnergize = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.TeamExtraEnergyByEnergizedFromMe)
                .FirstOrDefault();
            List<PlayerGameSummary> sortedPlayersEnemiesSightedPerTurn = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.EnemiesSightedPerTurn)
                .ToList();
            List<PlayerGameSummary> sortedPlayersFreelancerStats = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.FreelancerStats.OrderByDescending(i => i).FirstOrDefault())
                .ToList();
            List<PlayerGameSummary> sortedPlayersDamageDealtPerTurn = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetDamageDealtPerTurn())
                .ToList();
            List<PlayerGameSummary> sortedPlayersDamageEfficiency = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.DamageEfficiency)
                .ToList();
            List<PlayerGameSummary> sortedPlayersDamageDonePerLife = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetDamageDonePerLife())
                .ToList();
            List<PlayerGameSummary> sortedPlayersDamageTakenPerLife = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetDamageTakenPerLife())
                .ToList();
            List<PlayerGameSummary> sortedPlayersDodge = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.DamageAvoidedByEvades)
                .ToList();
            List<PlayerGameSummary> sortedPlayersCrowdControl = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.MovementDeniedByMe)
                .ToList();
            List<PlayerGameSummary> sortedPlayersHealedShielded = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.GetTotalHealingFromAbility() + p.TotalPlayerAbsorb)
                .ToList();
            List<PlayerGameSummary> sortedPlayersBoostTeamDamage = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.MyOutgoingExtraDamageFromEmpowered)
                .ToList();
            List<PlayerGameSummary> sortedPlayersBoostTeamEnergize = request.GameSummary.PlayerGameSummaryList
                .OrderByDescending(p => p.TeamExtraEnergyByEnergizedFromMe)
                .ToList();

            Dictionary<int, List<BadgeInfo>> badgeInfos = new Dictionary<int, List<BadgeInfo>>();

            foreach (PlayerGameSummary player in request.GameSummary.PlayerGameSummaryList)
            {
                List<BadgeInfo> playerBadgeInfos = new List<BadgeInfo>();

                if (player.NumAssists == 3) playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 1 });
                if (player.NumAssists == 4) playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 2 });
                if (player.NumAssists == 5) playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 3 });

                int playerIndexEnemiesSightedPerTurn =
                    sortedPlayersEnemiesSightedPerTurn.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexEnemiesSightedPerTurn >= 0 && highestEnemiesSightedPerTurn != null &&
                    highestEnemiesSightedPerTurn.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersEnemiesSightedPerTurn.Count;
                    double percentile = (totalPlayers - playerIndexEnemiesSightedPerTurn - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 6, 5, 4);
                }

                if (player.GetDamageDealtPerTurn() >= 20 && player.GetSupportPerTurn() >= 20 &&
                    player.GetTankingPerLife() >= 200) playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 9 });
                else if (player.GetDamageDealtPerTurn() >= 15 && player.GetSupportPerTurn() >= 15 &&
                         player.GetTankingPerLife() >= 150) playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 8 });
                else if (player.GetDamageDealtPerTurn() >= 10 && player.GetSupportPerTurn() >= 10 &&
                         player.GetTankingPerLife() >= 100) playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 7 });

                int playerIndexFreelancerStats = sortedPlayersFreelancerStats.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexFreelancerStats >= 0)
                {
                    int totalPlayers = sortedPlayersFreelancerStats.Count;
                    double percentile = (totalPlayers - playerIndexFreelancerStats - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 12, 11, 10);
                }

                if (highestDamagePerTurn != null && highestDamagePerTurn.PlayerId == player.PlayerId)
                    playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 13 });

                int playerIndexDamageDealtPerTurn =
                    sortedPlayersDamageDealtPerTurn.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexDamageDealtPerTurn >= 0 && highestDamagePerTurn != null &&
                    highestDamagePerTurn.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersDamageDealtPerTurn.Count;
                    double percentile = (totalPlayers - playerIndexDamageDealtPerTurn - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 16, 15, 14);
                }

                int playerIndexDamageEfficiency =
                    sortedPlayersDamageEfficiency.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexDamageEfficiency >= 0 && highestDamageEfficiency != null &&
                    highestDamageEfficiency.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersDamageEfficiency.Count;
                    double percentile = (totalPlayers - playerIndexDamageEfficiency - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 19, 18, 17);
                }

                int playerIndexDamageDonePerLife =
                    sortedPlayersDamageDonePerLife.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexDamageDonePerLife >= 0 && highestDamageDonePerLife != null &&
                    highestDamageDonePerLife.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersDamageDonePerLife.Count;
                    double percentile = (totalPlayers - playerIndexDamageDonePerLife - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 23, 22, 21);
                }

                int playerIndexDamageTakenPerLife =
                    sortedPlayersDamageTakenPerLife.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexDamageTakenPerLife >= 0 && highestDamageTakenPerLife != null &&
                    highestDamageTakenPerLife.PlayerId == player.PlayerId)
                {
                    if (highestDamageTakenPerLife != null && highestDamageTakenPerLife.PlayerId == player.PlayerId)
                        playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 20 });
                    int totalPlayers = sortedPlayersDamageTakenPerLife.Count;
                    double percentile = (totalPlayers - playerIndexDamageTakenPerLife - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 26, 25, 24);
                }


                int playerIndexDodge = sortedPlayersDodge.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexDodge >= 0 && highestDodge != null && highestDodge.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersDodge.Count;
                    double percentile = (totalPlayers - playerIndexDodge - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 29, 28, 27);
                }

                int playerIndexCrowdControl = sortedPlayersCrowdControl.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexCrowdControl >= 0 && highestCrowdControl != null &&
                    highestCrowdControl.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersCrowdControl.Count;
                    double percentile = (totalPlayers - playerIndexCrowdControl - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 32, 31, 30);
                }

                if (highestMitigated != null && highestMitigated.PlayerId == player.PlayerId)
                    playerBadgeInfos.Add(new BadgeInfo() { BadgeId = 33 });

                int playerIndexHealedShielded = sortedPlayersHealedShielded.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexHealedShielded >= 0 && highestHealingPlayer != null &&
                    highestHealingPlayer.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersHealedShielded.Count;
                    double percentile = (totalPlayers - playerIndexHealedShielded - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 36, 35, 34);
                }

                int playerIndexBoostTeamDamage = sortedPlayersBoostTeamDamage.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexBoostTeamDamage >= 0 && highestBoostTeamDamage != null &&
                    highestBoostTeamDamage.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersBoostTeamDamage.Count;
                    double percentile = (totalPlayers - playerIndexBoostTeamDamage - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 39, 38, 37);
                }

                int playerIndexBoostTeamEnergize =
                    sortedPlayersBoostTeamEnergize.FindIndex(p => p.PlayerId == player.PlayerId);

                if (playerIndexBoostTeamEnergize >= 0 && highestBoostTeamEnergize != null &&
                    highestBoostTeamEnergize.PlayerId == player.PlayerId)
                {
                    int totalPlayers = sortedPlayersBoostTeamEnergize.Count;
                    double percentile = (totalPlayers - playerIndexBoostTeamEnergize - 1) * 100.0 / totalPlayers;
                    AddBadge(playerBadgeInfos, percentile, 42, 41, 40);
                }

                badgeInfos[player.PlayerId] = playerBadgeInfos;
            }

            foreach (PlayerGameSummary player in request.GameSummary.PlayerGameSummaryList)
            {
                List<TopParticipantSlot> topParticipationEarned = new List<TopParticipantSlot>();

                if (highestHealingPlayer != null && highestHealingPlayer.PlayerId == player.PlayerId)
                {
                    topParticipationEarned.Add(TopParticipantSlot.Supportiest);
                }

                if (highestDamagePlayer != null && highestDamagePlayer.PlayerId == player.PlayerId)
                {
                    topParticipationEarned.Add(TopParticipantSlot.Deadliest);
                }

                if (highestDamageReceivedPlayer != null && highestDamageReceivedPlayer.PlayerId == player.PlayerId)
                {
                    topParticipationEarned.Add(TopParticipantSlot.Tankiest);
                }

                var playerBadgeCounts = badgeInfos
                    .GroupBy(b => b.Key)
                    .Select(g => new { PlayerId = g.Key, BadgeCount = g.Count() })
                    .OrderByDescending(x => x.BadgeCount);
                int maxBadgeCount = playerBadgeCounts.FirstOrDefault()?.BadgeCount ?? 0;
                int playerIdWithMostBadges = playerBadgeCounts
                    .Where(x => x.BadgeCount == maxBadgeCount)
                    .Select(x => x.PlayerId)
                    .FirstOrDefault();

                if (playerIdWithMostBadges == player.PlayerId)
                {
                    topParticipationEarned.Add(TopParticipantSlot.MostDecorated);
                }

                result.Add(new BadgeAndParticipantInfo
                {
                    PlayerId = player.PlayerId,
                    TeamId = player.IsInTeamA() ? Team.TeamA : Team.TeamB,
                    TeamSlot = player.TeamSlot,
                    BadgesEarned = badgeInfos[player.PlayerId],
                    TopParticipationEarned = topParticipationEarned,
                    GlobalPercentiles = new Dictionary<StatDisplaySettings.StatType, PercentileInfo>(),
                    FreelancerSpecificPercentiles = new Dictionary<int, PercentileInfo>(),
                    FreelancerPlayed = player.CharacterPlayed
                });
            }

            return result;
        }
        
        private static void AddBadge(List<BadgeInfo> badges, double percentile, int p80, int p75, int p50)
        {
            if (percentile > 80) badges.Add(new BadgeInfo { BadgeId = p80 });
            else if (percentile > 75) badges.Add(new BadgeInfo { BadgeId = p75 });
            else if (percentile > 50) badges.Add(new BadgeInfo { BadgeId = p50 });
        }
        
    }
}