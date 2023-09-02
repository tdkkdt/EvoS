using System.Collections.Generic;
using System.Linq;
using CentralServer.LobbyServer.Session;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;

namespace CentralServer.LobbyServer.Friend
{
    class FriendManager
    {
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
                // TODO We are all friends here for now
                Friends = SessionManager.GetOnlinePlayers()
                    .Where(id => id != accountId)
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
            return string.Empty;
        }

        public static PlayerUpdateStatusResponse OnPlayerUpdateStatusRequest(LobbyServerProtocol client, PlayerUpdateStatusRequest request)
        {
            // TODO: notify this client's friends the status change

            PlayerUpdateStatusResponse response = new PlayerUpdateStatusResponse()
            {
                AccountId = client.AccountId,
                StatusString = request.StatusString,
                ResponseId = request.RequestId
            };

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
    }
}
