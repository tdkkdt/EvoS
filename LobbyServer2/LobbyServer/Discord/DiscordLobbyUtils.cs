using System;
using System.Linq;
using CentralServer.LobbyServer.Group;
using CentralServer.LobbyServer.Session;
using Discord;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;

namespace CentralServer.LobbyServer.Discord
{
    public static class DiscordLobbyUtils
    {
        public static string GetMessageRecipients(ChatNotification notification, out string context)
        {
            context = null;
            switch (notification.ConsoleMessageType)
            {
                case ConsoleMessageType.GlobalChat:
                    return null;
                case ConsoleMessageType.WhisperChat:
                    return notification.RecipientHandle;
                case ConsoleMessageType.GameChat:
                case ConsoleMessageType.TeamChat:
                {
                    LobbyServerProtocol conn = SessionManager.GetClientConnection(notification.SenderAccountId);
                    LobbyServerPlayerInfo playerInfo = null;
                    string serverInfo = "";
                    if (conn.CurrentServer != null)
                    {
                        serverInfo = $"{conn.CurrentServer.Name} " +
                                     $"{new DateTime(conn.CurrentServer.GameInfo.CreateTimestamp):yyyy_MM_dd__HH_mm_ss}\n" +
                                     $"{conn.CurrentServer.GameInfo.GameConfig.Map} " +
                                     $"{conn.CurrentServer.GameInfo.GameConfig.GameType} ";
                        playerInfo = conn.CurrentServer.GetPlayerInfo(notification.SenderAccountId);
                    }

                    string team = "";
                    if (notification.ConsoleMessageType != ConsoleMessageType.GameChat && playerInfo != null)
                    {
                        team = $"{playerInfo.TeamId} ";
                    }
                    
                    context = $"{serverInfo}{team}";

                    if (conn.CurrentServer != null)
                    {
                        if (notification.ConsoleMessageType == ConsoleMessageType.GameChat)
                        {
                            return string.Join(", ",
                                conn.CurrentServer
                                    .GetPlayers()
                                    .Where(id => id != notification.SenderAccountId)
                                    .Select(id => DB.Get().AccountDao.GetAccount(id).Handle));
                        }
                        if (playerInfo != null)
                        {
                            return string.Join(", ",
                                conn.CurrentServer
                                    .GetPlayers(playerInfo.TeamId)
                                    .Where(id => id != notification.SenderAccountId)
                                    .Select(id => DB.Get().AccountDao.GetAccount(id).Handle));
                        }
                    }
                    return null;
                }
                case ConsoleMessageType.GroupChat:
                {
                    LobbyPlayerGroupInfo group = GroupManager.GetGroupInfo(notification.SenderAccountId);
                    return group != null
                        ? string.Join(", ",
                            group.Members
                                .Select(member => member.AccountID)
                                .Where(id => id != notification.SenderAccountId)
                                .Select(id => DB.Get().AccountDao.GetAccount(id).Handle))
                        : "<group>";
                }
                default:
                    return $"<{notification.ConsoleMessageType}>";
            }
        }
        
        public static Color GetColor(ConsoleMessageType messageType)
        {
            switch (messageType)
            {
                case ConsoleMessageType.GlobalChat:
                    return Color.Default;
                case ConsoleMessageType.WhisperChat:
                    return Color.Purple;
                case ConsoleMessageType.GameChat:
                    return Color.Gold;
                case ConsoleMessageType.TeamChat:
                    return Color.Blue;
                case ConsoleMessageType.GroupChat:
                    return Color.Green;
                default:
                    return Color.DarkRed;
            }
        }
        
        public static string BuildPlayerCountSummary(Status status)
        {
            return $"{status.totalPlayers} player{(status.totalPlayers != 1 ? "s" : "")} online, " +
                   $"{status.inQueue} in queue, {status.inGame} in game";
        }

        public static Status GetStatus()
        {
            int inGame = 0;
            int inQueue = 0;
            int online = 0;
            foreach (long accountId in SessionManager.GetOnlinePlayers())
            {
                LobbyServerProtocol conn = SessionManager.GetClientConnection(accountId);
                if (conn == null) continue;
                online++;
                if (conn.IsInQueue()) inQueue++;
                else if (conn.IsInGame()) inGame++;
            }

            Status status = new Status { totalPlayers = online, inGame = inGame, inQueue = inQueue };
            return status;
        }

        public struct Status
        {
            public int totalPlayers;
            public int inQueue;
            public int inGame;
        }
    }
}