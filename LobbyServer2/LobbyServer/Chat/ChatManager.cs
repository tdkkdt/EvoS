using System;
using CentralServer.LobbyServer.Group;
using CentralServer.LobbyServer.Session;
using EvoS.DirectoryServer.Inventory;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Misc;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using log4net;

namespace CentralServer.LobbyServer.Chat
{
    class ChatManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ChatManager));

        private static ChatManager _instance;

        public event Action<ChatNotification> OnGlobalChatMessage = delegate {};
        public event Action<ChatNotification> OnChatMessage = delegate {};

        public static ChatManager Get()
        {
            return _instance ??= new ChatManager();
        }
        
        private ChatManager()
        {
            SessionManager.OnPlayerConnected += Register;
            SessionManager.OnPlayerDisconnected += Unregister;
        }
        
        ~ChatManager()
        {
            SessionManager.OnPlayerConnected -= Register;
            SessionManager.OnPlayerDisconnected -= Unregister;
        }

        private void Register(LobbyServerProtocol conn)
        {
            conn.OnChatNotification += HandleChatNotification;
            conn.OnGroupChatRequest += HandleGroupChatRequest;
        }

        private void Unregister(LobbyServerProtocol conn)
        {
            // TODO unregister from every client on shutdown?
            conn.OnChatNotification -= HandleChatNotification;
            conn.OnGroupChatRequest -= HandleGroupChatRequest;
        }
        
        public void HandleChatNotification(LobbyServerProtocol conn, ChatNotification notification)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(conn.AccountId);
            ChatNotification message = new ChatNotification
            {
                SenderAccountId = conn.AccountId,
                SenderHandle = account.Handle,
                ResponseId = notification.RequestId,
                CharacterType = account.AccountComponent.LastCharacter,
                ConsoleMessageType = notification.ConsoleMessageType,
                Text = notification.Text,
                EmojisAllowed = InventoryManager.GetUnlockedEmojiIDs(conn.AccountId),
                DisplayDevTag = false,
            };

            LobbyServerPlayerInfo lobbyServerPlayerInfo = null;
            if (conn.CurrentServer != null)
            {
                lobbyServerPlayerInfo = conn.CurrentServer.GetPlayerInfo(conn.AccountId);
                if (lobbyServerPlayerInfo != null)
                {
                    message.SenderTeam = lobbyServerPlayerInfo.TeamId;
                }
                else
                {
                    log.Error($"{conn.AccountId} {account.Handle} attempted to use {notification.ConsoleMessageType} " +
                              $"but they are not in the game they are supposed to be in");
                }
            }
            
            switch (notification.ConsoleMessageType)
            {
                case ConsoleMessageType.GlobalChat:
                {
                    conn.Broadcast(message);
                    OnGlobalChatMessage(message);
                    break;
                }
                case ConsoleMessageType.WhisperChat:
                {
                    long? accountId = SessionManager.GetOnlinePlayerByHandle(notification.RecipientHandle);
                    if (accountId.HasValue)
                    {
                        message.RecipientHandle = notification.RecipientHandle;
                        SessionManager.GetClientConnection((long)accountId)?.Send(message);
                        conn.Send(message);
                    }
                    else
                    {
                        log.Warn($"{conn.AccountId} {account.Handle} failed to whisper to {notification.RecipientHandle}");
                    }
                    break;
                }
                case ConsoleMessageType.GameChat:
                {
                    if (conn.CurrentServer == null)
                    {
                        log.Warn($"{conn.AccountId} {account.Handle} attempted to use {notification.ConsoleMessageType} while not in game");
                        break;
                    }
                    foreach (long accountId in conn.CurrentServer.GetPlayers())
                    {
                        SessionManager.GetClientConnection(accountId)?.Send(message);
                    }
                    break;
                }
                case ConsoleMessageType.GroupChat:
                {
                    LobbyPlayerGroupInfo group = GroupManager.GetGroupInfo(conn.AccountId);
                    if (conn.CurrentServer == null) // TODO fix?
                    {
                        log.Error($"{conn.AccountId} {account.Handle} attempted to use {notification.ConsoleMessageType} while not in a group");
                        break;
                    }
                    foreach (UpdateGroupMemberData member in group.Members)
                    {
                        SessionManager.GetClientConnection(member.AccountID)?.Send(message);
                    }
                    break;
                }
                case ConsoleMessageType.TeamChat:
                {
                    if (conn.CurrentServer == null)
                    {
                        log.Warn($"{conn.AccountId} {account.Handle} attempted to use {notification.ConsoleMessageType} while not in game");
                        break;
                    }
                    if (lobbyServerPlayerInfo == null)
                    {
                        log.Error($"{conn.AccountId} {account.Handle} attempted to use {notification.ConsoleMessageType} " +
                                  $"but they are not in the game they are supposed to be in");
                        break;
                    }
                    foreach (long teammateAccountId in conn.CurrentServer.GetPlayers(lobbyServerPlayerInfo.TeamId))
                    {
                        SessionManager.GetClientConnection(teammateAccountId)?.Send(message);
                    }
                    break;
                }
                default:
                {
                    log.Error($"Console message type {notification.ConsoleMessageType} is not supported yet!");
                    log.Info(DefaultJsonSerializer.Serialize(notification));
                    break;
                }
            }

            OnChatMessage(message);
        }

        public void HandleGroupChatRequest(LobbyServerProtocol conn, GroupChatRequest request)
        {
            conn.Send(new GroupChatResponse
            {
                Text = request.Text,
                ResponseId = request.RequestId,
                Success = true
            });

            PersistedAccountData account = DB.Get().AccountDao.GetAccount(conn.AccountId);

            ChatNotification message = new ChatNotification
            {
                SenderAccountId = conn.AccountId,
                EmojisAllowed = request.RequestedEmojis,
                CharacterType = account.AccountComponent.LastCharacter,
                ConsoleMessageType = ConsoleMessageType.GroupChat,
                SenderHandle = account.Handle,
                Text = request.Text
            };

            foreach (long accountID in GroupManager.GetPlayerGroup(conn.AccountId).Members)
            {
                LobbyServerProtocol connection = SessionManager.GetClientConnection(accountID);
                connection.Send(message);
            }

            OnChatMessage(message);
        }
    }
}
