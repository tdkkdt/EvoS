using System;
using System.Collections.Generic;
using System.Linq;
using CentralServer.BridgeServer;
using CentralServer.LobbyServer.Group;
using CentralServer.LobbyServer.Session;
using EvoS.Framework;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;
using log4net;

namespace CentralServer.LobbyServer.Matchmaking;

public static class QueuePenaltyManager
{
    private static readonly ILog log = LogManager.GetLogger(typeof(QueuePenaltyManager));
    
    public static void IssueQueuePenalty(long accountId, BridgeServerProtocol server)
    {
        if (!LobbyConfiguration.GetMatchAbandoningPenalty()
            || server?.GameInfo?.GameConfig is null
            || server.GameInfo.GameConfig.GameType != GameType.PvP)
        {
            return;
        }

        if (server.TeamInfo.TeamPlayerInfo.Count(i => i.ReplacedWithBots) * 2 >=
            server.TeamInfo.TeamPlayerInfo.Count)
        {
            return;
        }
        if (server.ServerGameStatus != GameStatus.Stopped)
        {
            SetQueuePenalty(accountId, GameType.PvP, TimeSpan.FromSeconds(200));
        }
        else if (server.StopTime > DateTime.UtcNow)
        {
            SetQueuePenalty(accountId, GameType.PvP, DateTime.UtcNow.Subtract(server.StopTime).Add(TimeSpan.FromSeconds(30)));
        }
    }

    private static void SetQueuePenalty(long accountId, GameType gameType, TimeSpan timeSpan, bool overridePenalty = false)
    {
        PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
        if (account is null)
        {
            return;
        }
        account.AdminComponent.ActiveQueuePenalties ??= new Dictionary<GameType, QueuePenalties>();
        QueuePenalties penalties = account.AdminComponent.ActiveQueuePenalties.GetValueOrDefault(gameType);
        DateTime newTimeout = DateTime.UtcNow.Add(timeSpan);
        if (penalties is null)
        {
            penalties = new QueuePenalties();
            penalties.ResetQueueDodge();
        }

        if (overridePenalty || penalties.QueueDodgeBlockTimeout < newTimeout)
        {
            penalties.QueueDodgeBlockTimeout = newTimeout;
            log.Info($"{gameType} queue penalty for {account.Handle}: {timeSpan}");
        }

        account.AdminComponent.ActiveQueuePenalties[gameType] = penalties;
        DB.Get().AccountDao.UpdateAccount(account);

        GroupInfo playerGroup = GroupManager.GetPlayerGroup(accountId);
        if (playerGroup is not null)
        {
            MatchmakingManager.RemoveGroupFromQueue(playerGroup, true);
        }
    }

    public static LocalizationPayload CheckQueuePenalties(long accountId, GameType selectedGameType)
    {
        PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
        QueuePenalties queuePenalties = account?.AdminComponent.ActiveQueuePenalties?.GetValueOrDefault(selectedGameType);
        if (queuePenalties is not null && queuePenalties.QueueDodgeBlockTimeout > DateTime.UtcNow.Add(TimeSpan.FromSeconds(5)))
        {
            TimeSpan duration = queuePenalties.QueueDodgeBlockTimeout.Subtract(DateTime.UtcNow);
            if (selectedGameType == GameType.PvP && ServerManager.GetServerWithPlayer(accountId) is null)
            {
                log.Info($"{account.Handle}'s {duration} queue penalty is pardoned");
                SetQueuePenalty(accountId, selectedGameType, TimeSpan.Zero, true);
                return null;
            }
            
            LocalizationArg argDuration = LocalizationArg_TimeSpan.Create(duration);
            LocalizationPayload failure = LocalizationPayload.Create("QueueDodgerPenaltyAppliedToSelf", "Matchmaking", argDuration);
            log.Info($"{account.Handle} cannot join {selectedGameType} queue until {queuePenalties.QueueDodgeBlockTimeout}");
            
            GroupInfo group = GroupManager.GetPlayerGroup(accountId);
            if (group != null)
            {
                LocalizationArg argHandle = LocalizationArg_Handle.Create(account.Handle);
                foreach (long groupMember in group.Members)
                {
                    if (groupMember == accountId) continue;
                    LobbyServerProtocol conn = SessionManager.GetClientConnection(groupMember);
                    conn?.SendSystemMessage(
                        LocalizationPayload.Create("QueueDodgerPenaltyAppliedToGroupmate", "Matchmaking", argHandle, argDuration));
                }
            }
            
            // QueueDodgerPenaltyAppliedToSelf@Matchmaking,Text,0: timespan,,"Because you left the previous game after a match was found, you will not be allowed to queue for {0}."
            // QueueDodgerPenaltyAppliedToGroupmate@Matchmaking,Text,"0: playername, 1: timespan",,{0} left their previous game after a match was found. They cannot re-queue for {1}.
            // QueueDodgePenaltyBlocksQueueEntry@Matchmaking,Text,0: playername,,{0} is currently blocked from queueing because they left a recent game after a match was found.
            // QueueDodgerPenaltyCleared@Matchmaking,Text,0: gametype,,You have been cleared of any penalty for dodging a {0} game.
            // LeftTooManyActiveGamesToQueue@Matchmaking,Text,0: playername,,{0} has left too many games recently to be allowed to queue.
            // AllGroupMembersHaveLeftTooManyActiveGamesToQueue@Matchmaking,Text,,,At least one group member must eliminate their Leaver penalty.
            // CannotQueueUntilTimeout@Ranked,Text,0: datetime,,You cannot queue for this mode until {0} due to queue dodging.
            // CannotQueueMembersPenalized@Ranked,Text,0: members banned,,"You cannot queue for this mode because {0} have been penalized"
            // UnableToQueue@RankMode,Text,,,Unable To Queue

            return failure;
        }

        return null;
    }
}