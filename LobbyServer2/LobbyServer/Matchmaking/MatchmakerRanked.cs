using System;
using System.Collections.Generic;
using System.Linq;
using CentralServer.LobbyServer.Character;
using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;
using log4net;

namespace CentralServer.LobbyServer.Matchmaking;

public class MatchmakerRanked : MatchmakerBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(Matchmaker));

    public MatchmakerRanked(
        AccountDao accountDao,
        GameType gameType,
        GameSubType subType,
        string eloKey,
        Func<MatchmakingConfiguration> conf)
        : base(accountDao, gameType, subType, eloKey, conf)
    {
    }
    
    public MatchmakerRanked(
        GameType gameType,
        GameSubType subType,
        string eloKey,
        Func<MatchmakingConfiguration> conf)
        :this(DB.Get().AccountDao, gameType, subType, eloKey, conf)
    {
    }

    protected override bool FilterMatch(Match match, DateTime now)
    {
        double waitingTime = GetReferenceTime(match, now);
        int maxEloDiff = Conf.MaxTeamEloDifferenceStart +
                         Convert.ToInt32((Conf.MaxTeamEloDifference - Conf.MaxTeamEloDifferenceStart)
                                         * Math.Clamp(waitingTime / Conf.MaxTeamEloDifferenceWaitTime.TotalSeconds, 0, 1));

        float eloDiff = Math.Abs(match.TeamA.Elo - match.TeamB.Elo);
        bool result = eloDiff <= maxEloDiff;
        log.Debug($"{(result ? "A": "Disa")}llowed {match}, elo diff {eloDiff}/{maxEloDiff}, reference queue time {TimeSpan.FromSeconds(waitingTime)}");
        return result;
    }

    private static double GetReferenceTime(Match match, DateTime now)
    {
        int cutoff = int.Max(1, Convert.ToInt32(MathF.Floor(match.Groups.Count() / 2.0f))); // don't want to keep the first ones to queue waiting for too long
        double waitingTime = match.Groups
            .Select(g => (now - g.QueueTime).TotalSeconds)
            .Order()
            .TakeLast(cutoff)
            .Average();
        return waitingTime;
    }
    
    protected override float RankMatch(Match match, DateTime now, bool infoLog = false)
    {
        float teamEloDifferenceFactor = 1 - Cap(Math.Abs(match.TeamA.Elo - match.TeamB.Elo) / Conf.MaxTeamEloDifference);
        float teammateEloDifferenceAFactor = 1 - Cap((match.TeamA.MaxElo - match.TeamA.MinElo) / Conf.TeammateEloDifferenceWeightCap);
        float teammateEloDifferenceBFactor = 1 - Cap((match.TeamB.MaxElo - match.TeamB.MinElo) / Conf.TeammateEloDifferenceWeightCap);
        float teammateEloDifferenceFactor = (teammateEloDifferenceAFactor + teammateEloDifferenceBFactor) * 0.5f;
        double waitTime = Math.Sqrt(match.Groups.Select(g => Math.Pow((now - g.QueueTime).TotalSeconds, 2)).Average());
        float waitTimeFactor = Cap((float)(waitTime / Conf.WaitingTimeWeightCap.TotalSeconds));
        float teamCompositionFactor = (GetTeamCompositionFactor(match.TeamA) + GetTeamCompositionFactor(match.TeamB)) * 0.5f;
        float teamBlockFactor = (GetBlocksFactor(match.TeamA) + GetBlocksFactor(match.TeamB)) * 0.5f;
        float teamConfidenceBalanceFactor = GetTeamConfidenceBalanceFactor(match);
        
        // TODO balance max - min elo in the team
        // TODO recently canceled matches factor
        // TODO match-to-match variance factor
        // TODO accumulated wait time (time spent in queue for the last hour?)
        // TODO win history factor (too many losses - try not to put into a disadvantaged team)?
        // TODO non-linearity?
        
        // TODO if you are waiting for 20 minutes, you must be in the next game
        // TODO overrides line "we must have this player in the next match" & "we must start next match by specific time"

        float score =
            teamEloDifferenceFactor * Conf.TeamEloDifferenceWeight
            + teammateEloDifferenceFactor * Conf.TeammateEloDifferenceWeight
            + waitTimeFactor * Conf.WaitingTimeWeight
            + teamCompositionFactor * Conf.TeamCompositionWeight
            + teamBlockFactor * Conf.TeamBlockWeight
            + teamConfidenceBalanceFactor * Conf.TeamConfidenceBalanceWeight;
        
        string msg = $"Score {score:0.00} " +
                  $"(tElo:{teamEloDifferenceFactor:0.00}, " +
                  $"tmElo:{teammateEloDifferenceFactor:0.00}, " +
                  $"q:{waitTimeFactor:0.00}, " +
                  $"tComp:{teamCompositionFactor:0.00}, " +
                  $"blocks:{teamBlockFactor:0.00}, " +
                  $"tConf:{teamConfidenceBalanceFactor:0.00}" +
                  $") {match}";
        if (infoLog)
        {
            log.Info(msg);
        }
        else
        {
            log.Debug(msg);
        }

        return score;
    }

    private static float Cap(float factor)
    {
        return Math.Clamp(factor, 0, 1);
    }

    private static float GetTeamCompositionFactor(Match.Team team)
    {
        if (team.Groups.Count == 1)
        {
            return 1;
        }
        
        float score = 0;
        Dictionary<CharacterRole,int> roles = team.Accounts.Values
            .Select(acc => acc.AccountComponent.LastCharacter)
            .Select(ch => CharacterConfigs.Characters[ch].CharacterRole)
            .GroupBy(role => role)
            .ToDictionary(el => el.Key, el => el.Count());
        
        if (roles.ContainsKey(CharacterRole.Tank))
        {
            score += 0.3f;
        }
        if (roles.ContainsKey(CharacterRole.Support))
        {
            score += 0.3f;
        }
        if (roles.TryGetValue(CharacterRole.Assassin, out int flNum))
        {
            score += 0.2f * Math.Min(flNum, 2);
        }
        if (roles.TryGetValue(CharacterRole.None, out int fillNum))
        {
            score += 0.27f * Math.Min(fillNum, 4);
        }

        return Math.Min(score, 1);
    }

    private static float GetBlocksFactor(Match.Team team)
    {
        if (team.Groups.Count == 1)
        {
            return 1;
        }
        
        int totalBlocks = team.Accounts.Values
            .Select(acc => team.AccountIds.Count(accId => acc.SocialComponent.BlockedAccounts.Contains(accId)))
            .Sum();

        return 1 - Math.Min(totalBlocks * 0.125f, 1);
    }

    private float GetTeamConfidenceBalanceFactor(Match match)
    {
        int diff = Math.Abs(match.TeamA.Accounts.Values.Select(GetEloConfidenceLevel).Sum()
                            - match.TeamB.Accounts.Values.Select(GetEloConfidenceLevel).Sum());
        return 1 - Cap(diff * 0.33f);
    }

    private int GetEloConfidenceLevel(PersistedAccountData acc)
    {
        acc.ExperienceComponent.EloValues.GetElo(_eloKey, out _, out int eloConfLevel);
        return eloConfLevel;
    }
}