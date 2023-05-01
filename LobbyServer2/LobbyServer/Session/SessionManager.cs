using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CentralServer.BridgeServer;
using CentralServer.LobbyServer.Group;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Exceptions;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using log4net;

namespace CentralServer.LobbyServer.Session
{
    public static class SessionManager
    {
        private class SessionInfo
        {
            public LobbyServerPlayerInfo playerInfo;
            public LobbyServerProtocol conn;
            public LobbySessionInfo session;
        }
        
        private static readonly ConcurrentDictionary<long, SessionInfo> SessionInfos = new ConcurrentDictionary<long, SessionInfo>();

        private static readonly ILog log = LogManager.GetLogger(typeof(SessionManager));
        
        public static event Action<LobbyServerProtocol> OnPlayerConnected = delegate {};
        public static event Action<LobbyServerProtocol> OnPlayerDisconnected = delegate {};

        public static LobbyServerPlayerInfo OnPlayerConnect(LobbyServerProtocol client, RegisterGameClientRequest registerRequest)
        {
            if (registerRequest == null) return null;

            lock (SessionInfos)
            {
                
                if (registerRequest.SessionInfo == null) 
                    throw new RegisterGameException("Session Info not received");
                if (registerRequest.SessionInfo.SessionToken == null || registerRequest.SessionInfo.SessionToken == 0)
                    throw new RegisterGameException("Session Info not received"); ;
                
                LobbySessionInfo sessionInfo = GetSessionInfo(registerRequest.SessionInfo.AccountId);

                if (sessionInfo == null)
                    throw new RegisterGameException("Session not found. User not logged"); ; // Session not found
                if (sessionInfo.SessionToken != registerRequest.SessionInfo.SessionToken)
                    throw new RegisterGameException("This session is not valid anymore"); ; // Session token do not match

                long accountId = sessionInfo.AccountId;
            
                PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);

                client.AccountId = account.AccountId;
                client.UserName = account.UserName;
                client.SelectedGameType = GameType.PvP;
                client.SelectedSubTypeMask = 0;
                client.SessionToken = sessionInfo.SessionToken;

                LobbyServerPlayerInfo playerInfo = LoadPlayerInfo(account.AccountId);
                SessionInfos.TryRemove(client.AccountId, out _);
                SessionInfos.TryAdd(client.AccountId, new SessionInfo
                {
                    conn = client,
                    session = sessionInfo,
                    playerInfo = playerInfo
                });
                
                OnPlayerConnected(client);

                return playerInfo;
            }
        }

        public static LobbyServerPlayerInfo UpdateLobbyServerPlayerInfo(long accountId)
        {
            LobbyServerPlayerInfo playerInfo = LoadPlayerInfo(accountId);

            SessionInfos.TryGetValue(playerInfo.AccountId, out SessionInfo session);
            if (session != null)
            {
                session.playerInfo = playerInfo;
            }
                
            return playerInfo;
        }

        public static LobbyServerPlayerInfo LoadPlayerInfo(long accountId)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
            LobbyServerPlayerInfo playerInfo = new LobbyServerPlayerInfo
            {
                AccountId = account.AccountId,
                BannerID = account.AccountComponent.SelectedBackgroundBannerID == -1
                    ? 95
                    : account.AccountComponent.SelectedBackgroundBannerID, // patch for existing users: default is 95
                BotCanTaunt = false,
                CharacterInfo = LobbyCharacterInfo.Of(account.CharacterData[account.AccountComponent.LastCharacter]),
                ControllingPlayerId = 0,
                EffectiveClientAccessLevel = account.AccountComponent.AppliedEntitlements.ContainsKey("DEVELOPER_ACCESS")
                    ? ClientAccessLevel.Admin
                    : ClientAccessLevel.Full,
                EmblemID = account.AccountComponent.SelectedForegroundBannerID == -1
                    ? 65
                    : account.AccountComponent.SelectedForegroundBannerID, // patch for existing users: default is 65 
                Handle = account.Handle,
                IsGameOwner = true,
                IsLoadTestBot = false,
                IsNPCBot = false,
                PlayerId = 0,
                ReadyState = ReadyState.Unknown,
                ReplacedWithBots = false,
                RibbonID = account.AccountComponent.SelectedRibbonID,
                TitleID = account.AccountComponent.SelectedTitleID,
                TitleLevel = 1
            };
            return playerInfo;
        }

        public static void OnPlayerDisconnect(LobbyServerProtocol client)
        {
            lock (SessionInfos)
            {
                SessionInfos.TryGetValue(client.AccountId, out SessionInfo sessionInfo);
                // Sometimes on reconnections we first have the new connection and then we receive the previous disconnection
                // To avoid deleting the new connection, we check if the session token is the same
                if (sessionInfo != null && sessionInfo.session.SessionToken == client.SessionToken)
                {
                    SessionInfos.TryRemove(client.AccountId, out _);
                    // TODO: this sends to every player even if its in game and the disconnected player is not
                    //client.Broadcast(new ChatNotification() { Text = $"{client.UserName} disconnected", ConsoleMessageType = ConsoleMessageType.SystemMessage });
                }

                OnPlayerDisconnected(client);
            }
        }

        public static LobbyServerPlayerInfo GetPlayerInfo(long accountId)
        {
            SessionInfos.TryGetValue(accountId, out SessionInfo sessionInfo);
            return sessionInfo?.playerInfo;
        }

        public static LobbyServerProtocol GetClientConnection(long accountId)
        {
            SessionInfos.TryGetValue(accountId, out SessionInfo sessionInfo);
            return sessionInfo?.conn;
        }

        public static LobbySessionInfo GetSessionInfo(long accountId)
        {
            SessionInfos.TryGetValue(accountId, out SessionInfo sessionInfo);
            return sessionInfo?.session;
        }

        public static long? GetOnlinePlayerByHandle(string handle)
        {
            return SessionInfos.Values.FirstOrDefault(si => si.playerInfo?.Handle == handle)?.playerInfo.AccountId;
        }

        public static HashSet<long> GetOnlinePlayers()
        {
            return new HashSet<long>(SessionInfos.Keys);
        }

        public static LobbySessionInfo CreateSession(long accountId)
        {
            lock (SessionInfos) {
                // If we have a game with this accountId do not remove the session we need the info to be able to reconnect
                // Else remove it and create a new Session
                BridgeServerProtocol server = ServerManager.GetServerWithPlayer(accountId);
                if (server == null)
                {
                    SessionInfos.TryRemove(accountId, out _);
                }

                SessionInfos.TryGetValue(accountId, out SessionInfo session);
                LobbySessionInfo sessionInfo;
                PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);

                if (session == null)
                {
                    sessionInfo = new LobbySessionInfo
                    {
                        AccountId = accountId,
                        Handle = account.Handle,
                        UserName = account.Handle,
                        ConnectionAddress = "127.0.0.1",
                        BuildVersion = "STABLE-122-100",
                        LanguageCode = "",
                        FakeEntitlements = "",
                        ProcessCode = "",
                        ProcessType = ProcessType.AtlasReactor,
                        SessionToken = GenerateToken(account.Handle),
                        ReconnectSessionToken = GenerateToken(account.Handle),
                        Region = Region.EU,
                    };

                    // Add session for account
                    SessionInfos.TryAdd(accountId, new SessionInfo
                    {
                        session = sessionInfo
                    });
                }
                else
                {
                    sessionInfo = new LobbySessionInfo
                    {
                        AccountId = accountId,
                        Handle = account.Handle,
                        UserName = account.Handle,
                        ConnectionAddress = "127.0.0.1",
                        BuildVersion = "STABLE-122-100",
                        LanguageCode = "",
                        FakeEntitlements = "",
                        ProcessCode = "",
                        ProcessType = ProcessType.AtlasReactor,
                        SessionToken = session.session.SessionToken,
                        ReconnectSessionToken = GenerateToken(account.Handle), // This can be regenerated since we send ReconnectPlayerRequest that sends the new ReconnectSessionToken
                        Region = Region.EU,
                    };
                    // TODO why is it not saved
                }
                return sessionInfo;
            }
        }

        public static void CleanSessionAfterReconnect(long accountId)
        {
            lock (SessionInfos)
            {
                SessionInfos.TryGetValue(accountId, out SessionInfo client);
                if (client?.conn == null)
                {
                    return;
                }
                if (!client.conn.SessionCleaned)
                {
                    client.conn.SessionCleaned = true;
                    GroupManager.LeaveGroup(accountId, false);
                }
                client.conn.WebSocket.Close();
                SessionInfos.TryRemove(accountId, out _);
            }
        }

        private static long GenerateToken(string a)
        {
            int num = (Guid.NewGuid() + a).GetHashCode();
            if (num < 0)
            {
                num = -num;
            }
            return num;
        }
    }
}
