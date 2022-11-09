using System;
using System.Collections.Generic;
using CentralServer.LobbyServer.Character;
using CentralServer.LobbyServer.Config;
using CentralServer.LobbyServer.Friend;
using CentralServer.LobbyServer.Matchmaking;
using CentralServer.LobbyServer.Session;
using CentralServer.LobbyServer.Store;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Misc;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using log4net;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CentralServer.LobbyServer
{
    public class LobbyServerProtocol : LobbyServerProtocolBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LobbyServerProtocol));
        
        protected override void HandleOpen()
        {
            RegisterHandler(new EvosMessageDelegate<RegisterGameClientRequest>(HandleRegisterGame));
            RegisterHandler(new EvosMessageDelegate<OptionsNotification>(HandleOptionsNotification));
            RegisterHandler(new EvosMessageDelegate<CustomKeyBindNotification>(HandleCustomKeyBindNotification));
            RegisterHandler(new EvosMessageDelegate<PricesRequest>(HandlePricesRequest));
            RegisterHandler(new EvosMessageDelegate<PlayerUpdateStatusRequest>(HandlePlayerUpdateStatusRequest));
            RegisterHandler(new EvosMessageDelegate<PlayerMatchDataRequest>(HandlePlayerMatchDataRequest));
            RegisterHandler(new EvosMessageDelegate<SetGameSubTypeRequest>(HandleSetGameSubTypeRequest));
            RegisterHandler(new EvosMessageDelegate<PlayerInfoUpdateRequest>(HandlePlayerInfoUpdateRequest));
            RegisterHandler(new EvosMessageDelegate<CheckAccountStatusRequest>(HandleCheckAccountStatusRequest));
            RegisterHandler(new EvosMessageDelegate<CheckRAFStatusRequest>(HandleCheckRAFStatusRequest));
            RegisterHandler(new EvosMessageDelegate<ClientErrorSummary>(HandleClientErrorSummary));
            RegisterHandler(new EvosMessageDelegate<PreviousGameInfoRequest>(HandlePreviousGameInfoRequest));
            RegisterHandler(new EvosMessageDelegate<PurchaseTintRequest>(HandlePurchaseTintRequest));
            RegisterHandler(new EvosMessageDelegate<LeaveGameRequest>(HandleLeaveGameRequest));
            RegisterHandler(new EvosMessageDelegate<JoinMatchmakingQueueRequest>(HandleJoinMatchmakingQueueRequest));
            RegisterHandler(new EvosMessageDelegate<LeaveMatchmakingQueueRequest>(HandleLeaveMatchmakingQueueRequest));
            RegisterHandler(new EvosMessageDelegate<ChatNotification>(HandleChatNotification));


            /*
            RegisterHandler(new EvosMessageDelegate<PurchaseModResponse>(HandlePurchaseModRequest));
            RegisterHandler(new EvosMessageDelegate<PurchaseTauntRequest>(HandlePurchaseTauntRequest));
            RegisterHandler(new EvosMessageDelegate<PurchaseBannerBackgroundRequest>(HandlePurchaseBannerRequest));
            RegisterHandler(new EvosMessageDelegate<PurchaseBannerForegroundRequest>(HandlePurchaseEmblemRequest));
            RegisterHandler(new EvosMessageDelegate<PurchaseChatEmojiRequest>(HandlePurchaseChatEmoji));
            RegisterHandler(new EvosMessageDelegate<PurchaseLoadoutSlotRequest>(HandlePurchaseLoadoutSlot));
            */
        }

        protected override void HandleClose(CloseEventArgs e)
        {
            LobbyServerPlayerInfo playerInfo = SessionManager.GetPlayerInfo(this.AccountId);
            if (playerInfo != null)
            {
                log.Info(string.Format(Messages.PlayerDisconnected, this.UserName));
                SessionManager.OnPlayerDisconnect(this);
            }
            foreach (IWebSocketSession session in Sessions.Sessions)
            {
                ((LobbyServerProtocol)session)?.RefreshFriendList();
            }
        }

        public void RefreshFriendList()
        {
            Send(FriendManager.GetFriendStatusNotification(AccountId));
        }

        public void HandleRegisterGame(RegisterGameClientRequest request)
        {
            try
            {
                LobbyServerPlayerInfo playerInfo = SessionManager.OnPlayerConnect(this, request);

                if (playerInfo != null)
                {
                    log.Info(string.Format(Messages.LoginSuccess, this.UserName));
                    RegisterGameClientResponse response = new RegisterGameClientResponse
                    {
                        AuthInfo = new AuthInfo()
                        {
                            AccountId = AccountId,
                            Handle = playerInfo.Handle
                        },
                        SessionInfo = SessionManager.GetSessionInfo(request.AuthInfo.AccountId),
                        ResponseId = request.RequestId
                    };

                    Send(response);

                    SendLobbyServerReadyNotification();
                }
                else
                {
                    SendErrorResponse(new RegisterGameClientResponse(), request.RequestId, Messages.LoginFailed);
                }
            }
            catch (Exception e)
            {
                SendErrorResponse(new RegisterGameClientResponse(), request.RequestId, e);
            }
            foreach (IWebSocketSession session in Sessions.Sessions)
            {
                ((LobbyServerProtocol)session)?.RefreshFriendList();
            }
        }

        public void HandleOptionsNotification(OptionsNotification notification)
        {
        }

        public void HandleCustomKeyBindNotification(CustomKeyBindNotification notification)
        {
        }

        public void HandlePricesRequest(PricesRequest request)
        {
            PricesResponse response = StoreManager.GetPricesResponse();
            response.ResponseId = request.RequestId;
            Send(response);
        }

        public void HandlePlayerUpdateStatusRequest(PlayerUpdateStatusRequest request)
        {
            log.Info($"{this.UserName} is now {request.StatusString}");
            PlayerUpdateStatusResponse response = FriendManager.OnPlayerUpdateStatusRequest(this, request);

            Send(response);
        }

        public void HandlePlayerMatchDataRequest(PlayerMatchDataRequest request)
        {
            PlayerMatchDataResponse response = new PlayerMatchDataResponse()
            {
                MatchData = new List<PersistedCharacterMatchData>(),
                ResponseId = request.RequestId
            };

            Send(response);
        }

        public void HandleSetGameSubTypeRequest(SetGameSubTypeRequest request)
        {
            this.SelectedSubTypeMask = request.SubTypeMask;
            SetGameSubTypeResponse response = new SetGameSubTypeResponse() { ResponseId = request.RequestId };
            Send(response);
        }

        public void HandlePlayerInfoUpdateRequest(PlayerInfoUpdateRequest request)
        {
            LobbyPlayerInfoUpdate update = request.PlayerInfoUpdate;

            PersistedAccountData account = DB.Get().AccountDao.GetAccount(AccountId);
            if (update.CharacterType.HasValue)
            {
                account.AccountComponent.LastCharacter = update.CharacterType.Value;
            }
            CharacterComponent characterComponent = account.CharacterData[account.AccountComponent.LastCharacter].CharacterComponent;
            if (update.CharacterSkin.HasValue)
            {
                characterComponent.LastSkin = update.CharacterSkin.Value;
            }
            if (update.CharacterCards.HasValue)
            {
                characterComponent.LastCards = update.CharacterCards.Value;
            }
            if (update.CharacterMods.HasValue)
            {
                characterComponent.LastMods = update.CharacterMods.Value;
            }
            if (update.CharacterAbilityVfxSwaps.HasValue)
            {
                characterComponent.LastAbilityVfxSwaps = update.CharacterAbilityVfxSwaps.Value;
            }
            if (update.CharacterLoadoutChanges.HasValue)
            {
                characterComponent.CharacterLoadouts = update.CharacterLoadoutChanges.Value.CharacterLoadoutChanges;
            }
            if (update.LastSelectedLoadout.HasValue)
            {
                characterComponent.LastSelectedLoadout = update.LastSelectedLoadout.Value;
            }
            DB.Get().AccountDao.UpdateAccount(account);
            LobbyServerPlayerInfo playerInfo = SessionManager.UpdateLobbyServerPlayerInfo(AccountId);


            if (request.GameType != null && request.GameType.HasValue)
                SetGameType(request.GameType.Value);

            if (update.CharacterType != null && update.CharacterType.HasValue)
            {
                PlayerAccountDataUpdateNotification updateNotification = new PlayerAccountDataUpdateNotification()
                {
                    AccountData = account.CloneForClient()
                };
                Send(updateNotification);
            }

            if (update.AllyDifficulty != null && update.AllyDifficulty.HasValue)
                SetAllyDifficulty(update.AllyDifficulty.Value);
            if (update.ContextualReadyState != null && update.ContextualReadyState.HasValue)
                SetContextualReadyState(update.ContextualReadyState.Value);
            if (update.EnemyDifficulty != null && update.EnemyDifficulty.HasValue)
                SetEnemyDifficulty(update.EnemyDifficulty.Value);
            if (update.LastSelectedLoadout != null && update.LastSelectedLoadout.HasValue)
                SetLastSelectedLoadout(update.LastSelectedLoadout.Value);

            //Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
            
            PlayerInfoUpdateResponse response = new PlayerInfoUpdateResponse()
            {
                PlayerInfo = LobbyPlayerInfo.FromServer(playerInfo, 0, new MatchmakingQueueConfig()),
                CharacterInfo = playerInfo.CharacterInfo,
                OriginalPlayerInfoUpdate = update,
                ResponseId = request.RequestId
            };
            Send(response);
        }

        public void HandleCheckAccountStatusRequest(CheckAccountStatusRequest request)
        {
            CheckAccountStatusResponse response = new CheckAccountStatusResponse()
            {
                QuestOffers = new QuestOfferNotification() { OfferDailyQuest = false },
                ResponseId = request.RequestId
            };
            Send(response);
        }

        public void HandleCheckRAFStatusRequest(CheckRAFStatusRequest request)
        {
            CheckRAFStatusResponse response = new CheckRAFStatusResponse()
            {
                ReferralCode = "sampletext",
                ResponseId = request.RequestId
            };
            Send(response);
        }

        public void HandleClientErrorSummary(ClientErrorSummary request)
        {
        }

        public void HandlePreviousGameInfoRequest(PreviousGameInfoRequest request)
        {
            PreviousGameInfoResponse response = new PreviousGameInfoResponse()
            {
                PreviousGameInfo = null,
                ResponseId = request.RequestId
            };
            Send(response);
        }

        public void HandlePurchaseTintRequest(PurchaseTintRequest request)
        {
            Console.WriteLine("PurchaseTintRequest " + JsonConvert.SerializeObject(request));

            PurchaseTintResponse response = new PurchaseTintResponse()
            {
                Result = PurchaseResult.Success,
                CurrencyType = request.CurrencyType,
                CharacterType = request.CharacterType,
                SkinId = request.SkinId,
                TextureId = request.TextureId,
                TintId = request.TintId,
                ResponseId = request.RequestId
            };
            Send(response);

            SkinHelper sk = new SkinHelper();
            sk.AddSkin(request.CharacterType, request.SkinId, request.TextureId, request.TintId);
            sk.Save();
        }

        public void HandleLeaveGameRequest(LeaveGameRequest request)
        {
            Console.WriteLine("LeaveGameRequest " + JsonConvert.SerializeObject(request));

            LeaveGameResponse response = new LeaveGameResponse()
            {
                Success = true,
                ResponseId = request.RequestId
            };
            Send(response);

            if (CurrentServer != null)
            {
                CurrentServer.clients.Remove(this);
                GameStatusNotification notify = new GameStatusNotification()
                {
                    GameServerProcessCode = CurrentServer.ProcessCode,
                    GameStatus = GameStatus.Stopped // TODO check if there is a better way to make client leave mid-game
                };
                Send(notify);
                CurrentServer = null; // we will probably want to save it somewhere for reconnection
            }
        }
        
        public void HandleJoinMatchmakingQueueRequest(JoinMatchmakingQueueRequest request)
        {
            log.Info($"{this.UserName} joined {request.GameType} queue ");

            // Send response to the calling request
            Send(new JoinMatchmakingQueueResponse { LocalizedFailure = null, ResponseId = request.RequestId });

            // Add player to the selected queue
            MatchmakingManager.AddToQueue(request.GameType, this);
        }

        public void HandleLeaveMatchmakingQueueRequest(LeaveMatchmakingQueueRequest request)
        {
            log.Error("Code not implemented yet for LeaveMatchmakingQueueRequest, must remove from queue");
            Send(new LeaveMatchmakingQueueResponse() { ResponseId = request.RequestId });
        }

        public void HandleChatNotification(ChatNotification notification)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(AccountId);
            ChatNotification message = new ChatNotification
            {
                Text = notification.Text,
                ConsoleMessageType = notification.ConsoleMessageType,
                CharacterType = account.AccountComponent.LastCharacter, // TODO in-game info
                DisplayDevTag = false,
                EmojisAllowed = new List<int>(), // TODO emoji
                SenderAccountId = AccountId,
                SenderHandle = account.Handle,
                SenderTeam = Team.Invalid, // TODO in-game info
                ResponseId = notification.RequestId
            };
            switch (notification.ConsoleMessageType)
            {
                case ConsoleMessageType.GlobalChat:
                    Broadcast(message);
                    break;
                case ConsoleMessageType.WhisperChat:
                    long? accountId = SessionManager.GetOnlinePlayerByHandle(notification.RecipientHandle);
                    if (accountId.HasValue)
                    {
                        SessionManager.GetClientConnection((long)accountId).Send(message);
                    }
                    else
                    {
                        log.Warn($"{AccountId} {account.Handle} failed to whisper to {notification.RecipientHandle}");
                    }
                    break;
                case ConsoleMessageType.GameChat:
                case ConsoleMessageType.GroupChat:
                case ConsoleMessageType.TeamChat:
                    log.Error($"Console message type {notification.ConsoleMessageType} is not supported yet!");
                    log.Info(DefaultJsonSerializer.Serialize(notification));
                    Send(new ChatNotification
                    {
                        SenderHandle = "system",
                        Text = $"Sorry, {notification.ConsoleMessageType} is not supported yet!",
                        ConsoleMessageType = ConsoleMessageType.WhisperChat,
                        ResponseId = notification.RequestId
                    });
                    break;
                default:
                    log.Error($"Console message type {notification.ConsoleMessageType} is not supported yet!");
                    log.Info(DefaultJsonSerializer.Serialize(notification));
                    break;
            }
        }
    }
}
