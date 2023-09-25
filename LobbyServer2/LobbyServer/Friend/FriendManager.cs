using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CentralServer.LobbyServer.Session;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;

namespace CentralServer.LobbyServer.Friend
{
    public enum PlayerOnlineStatus
    {
        Online,
        Away,
        Busy
    }
    
    class FriendManager
    {
        private static readonly Dictionary<string, PlayerOnlineStatus> StatusMap = Enum
            .GetValues<PlayerOnlineStatus>()
            .ToDictionary((PlayerOnlineStatus e) => e.ToString());
        
        private static readonly ConcurrentDictionary<long, byte> PendingUpdate = new ConcurrentDictionary<long, byte>();
        
        public static FriendStatusNotification GetFriendStatusNotification(long accountId)
        {
            FriendStatusNotification notification = new FriendStatusNotification()
            {
                FriendList = GetFriendList(accountId)
            };

            return notification;
        }

        public static FriendList GetFriendList(long accountId)
        {
            SocialComponent socialComponent = DB.Get().AccountDao.GetAccount(accountId)?.SocialComponent;
            FriendList friendList = new FriendList
            {
                Friends = GetFriends(accountId)
                    .Select(id => DB.Get().AccountDao.GetAccount(id))
                    .ToDictionary(acc => acc.AccountId,
                        acc =>
                        {
                            LobbyServerProtocol conn = SessionManager.GetClientConnection(acc.AccountId);
                            return new FriendInfo
                            {
                                FriendAccountId = acc.AccountId,
                                FriendHandle = acc.Handle,
                                FriendStatus = socialComponent?.IsBlocked(acc.AccountId) == true ? FriendStatus.Blocked : FriendStatus.Friend,
                                IsOnline = conn != null,
                                StatusString = GetStatusString(conn),
                                // FriendNote = 
                                BannerID = acc.AccountComponent.SelectedBackgroundBannerID,
                                EmblemID = acc.AccountComponent.SelectedForegroundBannerID,
                                TitleID = acc.AccountComponent.SelectedTitleID,
                                TitleLevel = acc.AccountComponent.TitleLevels.GetValueOrDefault(acc.AccountComponent.SelectedTitleID, 0),
                                RibbonID = acc.AccountComponent.SelectedRibbonID,
                            };
                        }),
                IsDelta = false
            };

            return friendList;
        }

        public static List<long> GetFriends(long accountId)
        {
            // TODO We are all friends here for now
            return SessionManager.GetOnlinePlayers()
                .Where(id => id != accountId)
                .ToList();
        }

        public static string GetStatusString(LobbyServerProtocol client)
        {
            if (client == null)
            {
                return "Offline";
            }
            if (client.IsInGame())
            {
                return client.IsInCharacterSelect() ? "Character Select" : "In Game";
            }
            if (client.IsInQueue())
            {
                return "Queued";
            }
            if (client.IsInGroup())
            {
                return "GroupChatRoom";  // No localization for "In Group" status so we have to borrow this one
            }
            return client.Status.ToString();
        }

        public static PlayerUpdateStatusResponse OnPlayerUpdateStatusRequest(LobbyServerProtocol client, PlayerUpdateStatusRequest request)
        {
            bool success = StatusMap.TryGetValue(request.StatusString, out PlayerOnlineStatus status);
            PlayerUpdateStatusResponse response = new PlayerUpdateStatusResponse()
            {
                AccountId = client.AccountId,
                StatusString = request.StatusString,
                ResponseId = request.RequestId,
                Success = success
            };
            
            if (success)
            {
                client.Status = status;
                MarkForUpdate(client);
            }

            return response;
        }

        public static string GetFailTerm(FriendOperation op)
        {
            switch (op)
            {
                case FriendOperation.Accept:
                    return "FailedFriendAccept";
                case FriendOperation.Add:
                    return "FailedFriendAdd";
                case FriendOperation.Reject:
                    return "FailedFriendReject";
                case FriendOperation.Remove:
                    return "FailedFriendRemove";
                case FriendOperation.Block:
                    return "FailedFriendBlock";
                default:
                    return null;
            }
        }

        public static void MarkForUpdate(LobbyServerProtocol client)
        {
            MarkForUpdate(client.AccountId);
        }

        public static void MarkForUpdate(long accountId)
        {
            lock (PendingUpdate)
            {
                PendingUpdate.TryAdd(accountId, 0);
            }
        }

        public static HashSet<long> GetAndResetPendingUpdate()
        {
            lock (PendingUpdate)
            {
                HashSet<long> result = PendingUpdate.Keys.ToHashSet();
                PendingUpdate.Clear();
                return result;
            }
        }
    }
}
