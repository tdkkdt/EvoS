using System;
using System.Collections.Generic;
using System.Linq;
using CentralServer.LobbyServer;
using CentralServer.LobbyServer.Session;
using CentralServer.LobbyServer.Utils;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Misc;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using log4net;

namespace CentralServer.BridgeServer;

public abstract class GameServerBase : WebSocketBehaviorBase<AllianceMessageBase>
{
    // TODO LOW Compose WebSocketBehaviors instead of inheriting? Custom games don't need them.
    // Probably we should make MatchOrchestrator and CustomGame related entities referencing BridgeServerProtocol which handles networking only
    
    private static readonly ILog log = LogManager.GetLogger(typeof(GameServerBase));
    
    public LobbyGameInfo GameInfo { protected set; get; }
    public LobbyServerTeamInfo TeamInfo { protected set; get; } = new LobbyServerTeamInfo() { TeamPlayerInfo = new List<LobbyServerPlayerInfo>() };
    public string ProcessCode { set; get; }  // TODO merge with GameInfo.GameServerProcessCode
    public string Name { protected set; get; }

    // TODO sync with GameInfo.GameStatus or get rid of it (GameInfo can be null)
    public GameStatus ServerGameStatus { get; protected set; } = GameStatus.None;

    // TODO there can be multiple
    public LobbyServerPlayerInfo GetPlayerInfo(long accountId)
    {
        return TeamInfo.TeamPlayerInfo.Find(p => p.AccountId == accountId && !p.IsRemoteControlled);
    }

    public LobbyServerPlayerInfo GetPlayerById(int playerId)
    {
        return TeamInfo.TeamPlayerInfo.Find(p => p.PlayerId == playerId);
    }

    public IEnumerable<long> GetPlayers(Team team)
    {
        return from p in TeamInfo.TeamInfo(team) select p.AccountId;
    }

    public IEnumerable<long> GetPlayers()
    {
        return from p in TeamInfo.TeamPlayerInfo select p.AccountId;
    }

    public List<LobbyServerProtocol> GetClients()
    {
        List<LobbyServerProtocol> clients = new List<LobbyServerProtocol>();

        if (TeamInfo?.TeamPlayerInfo == null)
        {
            return clients;
        }

        HashSet<long> accountIds = new HashSet<long>();
        foreach (LobbyServerPlayerInfo player in TeamInfo.TeamPlayerInfo)
        {
            if (player.IsSpectator
                || player.IsNPCBot
                || player.ReplacedWithBots
                || accountIds.Contains(player.AccountId))
            {
                continue;
            }
            LobbyServerProtocol client = SessionManager.GetClientConnection(player.AccountId);
            if (client != null)
            {
                accountIds.Add(client.AccountId);
                clients.Add(client);
            }
        }

        return clients;
    }

    public void ForceReady()
    {
        TeamInfo.TeamPlayerInfo.ForEach(p => p.ReadyState = ReadyState.Ready);
    }

    public void ForceUnReady()
    {
        TeamInfo.TeamPlayerInfo.ForEach(p => p.ReadyState = ReadyState.Unknown);
    }

    public void OnAccountVisualsUpdated(long accountId)
    {
        LobbyServerPlayerInfo serverPlayerInfo = GetPlayerInfo(accountId);
        PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
        if (account != null)
        {
            serverPlayerInfo.TitleID = account.AccountComponent.SelectedTitleID;
            serverPlayerInfo.TitleLevel = account.AccountComponent.TitleLevels.GetValueOrDefault(account.AccountComponent.SelectedTitleID, 1);
            serverPlayerInfo.BannerID = account.AccountComponent.SelectedBackgroundBannerID;
            serverPlayerInfo.EmblemID = account.AccountComponent.SelectedForegroundBannerID;
            serverPlayerInfo.RibbonID = account.AccountComponent.SelectedRibbonID;
        }
    }

    public abstract bool UpdateCharacterInfo(
        long accountId,
        LobbyCharacterInfo characterInfo,
        LobbyPlayerInfoUpdate update);

    public abstract void DisconnectPlayer(long accountId);

    public void OnPlayerUsedGGPack(long accountId)
    {
        GameInfo.ggPackUsedAccountIDs.TryGetValue(accountId, out int ggPackUsedAccountIDs);
        GameInfo.ggPackUsedAccountIDs[accountId] = ggPackUsedAccountIDs + 1;
    }

    public abstract void SetSecondaryCharacter(long accountId, int playerId, CharacterType characterType);
    
    protected override AllianceMessageBase DeserializeMessage(byte[] data, out int callbackId)
    {
        return BridgeMessageSerializer.DeserializeMessage(data, out callbackId);
    }

    public void SendGameAssignmentNotification(LobbyServerProtocol client, bool reconnection = false)
    {
        LobbyServerPlayerInfo playerInfo = GetPlayerInfo(client.AccountId);
        GameAssignmentNotification notification = new GameAssignmentNotification
        {
            GameInfo = GameInfo,
            GameResult = GameInfo.GameResult,
            Observer = false,
            PlayerInfo = LobbyPlayerInfo.FromServer(playerInfo, 0, new MatchmakingQueueConfig()),
            Reconnection = reconnection,
            GameplayOverrides = GameConfig.GetGameplayOverrides()
        };

        client.Send(notification);
    }

    public void SendGameInfoNotifications()
    {
        GameInfo.ActivePlayers = TeamInfo.TeamPlayerInfo.Count;
        GameInfo.UpdateTimestamp = DateTime.UtcNow.Ticks;
        foreach (long player in GetPlayers())
        {
            LobbyServerProtocol playerConnection = SessionManager.GetClientConnection(player);
            if (playerConnection != null)
            {
                SendGameInfo(playerConnection);
            }
        }
    }

    public void SendGameInfo(LobbyServerProtocol playerConnection, GameStatus gamestatus = GameStatus.None)
    {
        // TODO do not mutate on send
        if (gamestatus != GameStatus.None)
        {
            GameInfo.GameStatus = gamestatus;
        }

        LobbyServerPlayerInfo playerInfo = GetPlayerInfo(playerConnection.AccountId);
        GameInfoNotification notification = new GameInfoNotification
        {
            GameInfo = GameInfo,
            TeamInfo = LobbyTeamInfo.FromServer(TeamInfo, 0, new MatchmakingQueueConfig()),
            PlayerInfo = LobbyPlayerInfo.FromServer(playerInfo, 0, new MatchmakingQueueConfig())
        };

        playerConnection.Send(notification);
    }

    protected void SendGameAssignmentNotification(long accountId, bool reconnection = false)
    {
        LobbyServerProtocol client = SessionManager.GetClientConnection(accountId);
        if (client is null)
        {
            log.Error($"Failed to send game assignment to {LobbyServerUtils.GetHandle(accountId)}");
            return;
        }
        SendGameAssignmentNotification(client, reconnection);
    }

    public virtual void SetPlayerReady(long accountId)
    {
        GetPlayerInfo(accountId).ReadyState = ReadyState.Ready;
    }

    public virtual void SetPlayerUnReady(long accountId)
    {
        GetPlayerInfo(accountId).ReadyState = ReadyState.Unknown;
    }
}