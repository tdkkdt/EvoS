using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CentralServer.LobbyServer.Matchmaking;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using Moq;

namespace BenchmarkMatchMaker;

static class Program {
    public static void Main() {
#if DEBUG
        var b = new  Benchmark
        {
            Players = 18
        };
        b.IterationSetup();
        Console.WriteLine(b.Calc());
#else
        BenchmarkRunner.Run<Benchmark>();
#endif
    }
}

public class Benchmark {
    [Params(5, 10, 15)]
    public int Players { get; set; }

    public Matchmaker Matchmaker { get; set; }
    public DateTime Now { get; set; }
    public List<Matchmaker.MatchmakingGroup> QueuedGroups { get; set; }

    [IterationSetup]
    public void IterationSetup() {
        Matchmaker = MakeMatchmaker(
            new MatchmakingConfiguration
            {
                MaxTeamEloDifferenceStart = 2000,
                MaxTeamEloDifference = 2000,
            }
        );
        Now = DateTime.UtcNow;
        QueuedGroups = [];
        for (var i = 1; i <= Players; i++) {
            QueuedGroups.Add(new Matchmaker.MatchmakingGroup(i, [i], Now));
        }
    }

    [Benchmark]
    public long Calc() {
        List<Matchmaker.Match> matchesRanked = Matchmaker.GetMatchesRanked(QueuedGroups, Now);
        return matchesRanked.Count;
    }

    private const string ELO_KEY = "elo_key";
    
    private static PersistedAccountData MakeAccount(long accId, string username, float elo, int eloConfidenceLevel)
    {
        var acc = new PersistedAccountData
        {
            AccountId = accId,
            UserName = username,
            Handle = $"{username}#{accId}",
            ExperienceComponent = new ExperienceComponent
            {
                EloValues = new EloValues()
            },
            AccountComponent = new AccountComponent
            {
                LastCharacter = CharacterType.PendingWillFill
            },
            SocialComponent = new SocialComponent
            {
                BlockedAccounts = new HashSet<long>()
            },
        };
        
        acc.ExperienceComponent.EloValues.UpdateElo(ELO_KEY, elo, eloConfidenceLevel);

        return acc;
    }    
    
    private static readonly Dictionary<long, PersistedAccountData> ACCOUNTS = new()
    {
        { 1, MakeAccount(1, "Psycho", 2260, 2) },
        { 2, MakeAccount(2, "Donut", 2180, 2) },
        { 3, MakeAccount(3, "Kid", 2070, 2) },
        { 4, MakeAccount(4, "Goose", 2050, 2) },
        { 5, MakeAccount(5, "Joke", 2050, 2) },
        { 6, MakeAccount(6, "Dozen", 2040, 2) },
        { 7, MakeAccount(7, "Script", 1990, 2) },
        { 8, MakeAccount(8, "Dream", 1960, 2) },
        { 9, MakeAccount(9, "Dolly", 1910, 2) },
        { 10, MakeAccount(10, "Darkness", 1860, 2) },
        { 11, MakeAccount(11, "Assault", 1560, 2) },
        { 12, MakeAccount(12, "Grounded", 1550, 2) },
        { 13, MakeAccount(13, "Crossbow", 1500, 2) },
        { 14, MakeAccount(14, "Hammer", 1500, 1) },
        { 15, MakeAccount(15, "Everyone", 1500, 1) },
        { 16, MakeAccount(16, "Cater", 1500, 0) },
        { 17, MakeAccount(17, "Crackle", 1500, 0) },
        { 18, MakeAccount(18, "Doe", 1460, 2) },
        { 19, MakeAccount(19, "Adult", 1450, 2) },
        { 20, MakeAccount(20, "Geek", 1340, 2) },
        { 21, MakeAccount(21, "Pie", 1230, 2) },
        { 22, MakeAccount(22, "Medic", 1060, 2) },
    };
    
    private static Matchmaker MakeMatchmaker(MatchmakingConfiguration conf)
    {
        var dao = MockAccountDao(ACCOUNTS);
        Matchmaker matchmaker = new MatchmakerRanked(
            dao,
            GameType.PvP,
            new GameSubType
            {
                LocalizedName = "Test",
                TeamAPlayers = 4,
                TeamBPlayers = 4
            },
            ELO_KEY,
            () => conf);
        return matchmaker;
    }
    
    private static AccountDao MockAccountDao(Dictionary<long, PersistedAccountData> accounts)
    {
        var mock = new Mock<AccountDao>();
        mock
            .Setup(dao => dao.GetAccount(It.IsAny<long>()))
            .Returns((long accId) => accounts[accId]);
        return mock.Object;
    }    
}