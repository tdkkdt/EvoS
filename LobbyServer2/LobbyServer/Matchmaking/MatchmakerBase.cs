using System;
using System.Collections.Generic;
using System.Linq;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;

namespace CentralServer.LobbyServer.Matchmaking;

public abstract class MatchmakerBase: Matchmaker
{
    protected readonly AccountDao _accountDao;
    protected readonly string _eloKey;
    
    protected MatchmakerBase(
        AccountDao accountDao,
        GameType gameType,
        GameSubType subType,
        string eloKey)
        : base(gameType, subType)
    {
        _accountDao = accountDao;
        _eloKey = eloKey;
    }

    protected override IEnumerable<Match> FindMatches(List<MatchmakingGroup> queuedGroups)
    {
        return FindMatches(new MatchScratch(_subType), queuedGroups, 0, _eloKey);
    }

    private IEnumerable<Match> FindMatches(
        MatchScratch matchScratch,
        List<MatchmakingGroup> queuedGroups,
        int qi,
        string eloKey
    ) {
        for (var i = qi; i < queuedGroups.Count; i++) {
            var groupInfo = queuedGroups[i];
            var maxTeamIndex = matchScratch.Count > 0 ? matchScratch.TeamsCount : 1;
            for (var teamIndex = 0; teamIndex < maxTeamIndex; teamIndex++) {
                if (matchScratch.Push(teamIndex, groupInfo)) {
                    if (matchScratch.IsMatch()) {
                        yield return matchScratch.ToMatch(_accountDao, eloKey);
                    }
                    else {
                        foreach (Match match in FindMatches(matchScratch, queuedGroups, i + 1, eloKey)) {
                            yield return match;
                        }
                    }
                    matchScratch.Pop(teamIndex);
                }
            }
        }
    }

    class MatchScratch
    {
        class Team
        {
            private readonly int _capacity;
            
            private readonly List<MatchmakingGroup> _groups = new(5);
            private int _size;

            public Team(int capacity)
            {
                _capacity = capacity;
            }

            public Team(Team other) : this(other._capacity)
            {
                _groups = other._groups.ToList();
                _size = other._size;
            }

            public bool IsFull => _size == _capacity;
            public List<MatchmakingGroup> Groups => _groups;

            public int GetHash()
            {
                return _groups
                    .SelectMany(g => g.Members)
                    .Order()
                    .Select(accountId => accountId.GetHashCode())
                    .Aggregate(17, (a, b) => a * 31 + b);
            }

            public bool Push(MatchmakingGroup groupInfo)
            {
                if (_capacity <= _size || _capacity - _size < groupInfo.Players)
                {
                    return false;
                }
                _size += groupInfo.Players;
                _groups.Add(groupInfo);
                return true;
            }

            public bool Pop(out long groupId)
            {
                groupId = -1;
                if (_groups.Count <= 0)
                {
                    return false;
                }
                MatchmakingGroup groupInfo = _groups[^1];
                _size -= groupInfo.Players;
                _groups.RemoveAt(_groups.Count - 1);
                groupId = groupInfo.GroupID;
                return true;
            }

            public override string ToString()
            {
                return
                    $"groups {string.Join(",", _groups.Select(g => g.GroupID))} " +
                    $"<{string.Join(",", _groups.SelectMany(g => g.Members))}>";
            }

            public int Count => _size;
        }
        
        private readonly Team _teamA;
        private readonly Team _teamB;
        private readonly HashSet<long> _usedGroupIds = new(10);

        public MatchScratch(GameSubType subType)
        {
            _teamA = new Team(subType.TeamAHumanPlayers);
            _teamB = new Team(subType.TeamBHumanPlayers);
        }

        private MatchScratch(Team teamA, Team teamB, HashSet<long> usedGroupIds)
        {
            _teamA = teamA;
            _teamB = teamB;
            _usedGroupIds = usedGroupIds;
        }

        public Match ToMatch(AccountDao accountDao, string eloKey)
        {
            return new Match(accountDao, _teamA.Groups.ToList(), _teamB.Groups.ToList(), eloKey);
        }

        public long GetHash()
        {
            int a = _teamA.GetHash();
            int b = _teamB.GetHash();
            return (long)Math.Min(a, b) << 32 | (uint)Math.Max(a, b);
        }

        public bool Push(int teamIndex, MatchmakingGroup groupInfo) {
            var team = teamIndex == 0 ? _teamA : _teamB;
            if (_usedGroupIds.Contains(groupInfo.GroupID) || !team.Push(groupInfo)) {
                return false;
            }
            _usedGroupIds.Add(groupInfo.GroupID);
            return true;
        }

        public void Pop(int teamIndex) {
            var team = teamIndex == 0 ? _teamA : _teamB;
            if (!team.Pop(out var groupId)) {
                throw new Exception("Matchmaking failure");
            }
            _usedGroupIds.Remove(groupId);
        }
        
        public bool IsMatch()
        {
            return _teamA.IsFull && _teamB.IsFull;
        }

        public override string ToString()
        {
            return $"{_teamA} vs {_teamB}";
        }

        public int Count => _teamA.Count + _teamB.Count;

        public int TeamsCount =>  2;
    }
}