using System;
using System.Collections.Generic;
using System.Linq;
using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;
using log4net;

namespace CentralServer.LobbyServer.Matchmaking;

public class MatchmakerSingleGroup : Matchmaker
{
    private static readonly ILog log = LogManager.GetLogger(typeof(MatchmakerSingleGroup));

    private readonly AccountDao _accountDao;
    
    public MatchmakerSingleGroup(AccountDao accountDao, GameType gameType, GameSubType subType) 
        : base(gameType, subType)
    {
        _accountDao = accountDao;
    }
    
    public MatchmakerSingleGroup(GameType gameType, GameSubType subType)
        : this(DB.Get().AccountDao, gameType, subType)
    {
    }
        
    public override List<Match> GetMatchesRanked(List<MatchmakingGroup> queuedGroups, DateTime now)
    {
        MatchmakingGroup group = queuedGroups
            .OrderBy(g => g.QueueTime)
            .FirstOrDefault(g => g.Players <= _subType.TeamAPlayers);
        if (group is null)
        {
            return new();
        }

        Match bestMatch = new Match(_accountDao, new() { group }, new(), string.Empty);
        
        log.Info($"Best match: {bestMatch}");
        return new() { bestMatch };
    }
}