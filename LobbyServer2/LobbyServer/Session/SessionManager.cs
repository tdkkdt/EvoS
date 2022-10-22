using CentralServer.LobbyServer.Character;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Logging;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EvoS.Framework.DataAccess;

namespace CentralServer.LobbyServer.Session
{
    public static class SessionManager
    {
        private static ConcurrentDictionary<long, LobbyServerPlayerInfo> ActivePlayers = new ConcurrentDictionary<long, LobbyServerPlayerInfo>();// key: AccountID
        private static ConcurrentDictionary<long, long> SessionTokenAccountIDCache = new ConcurrentDictionary<long, long>(); // key: SessionToken, value: AccountID
        private static ConcurrentDictionary<long, LobbyServerProtocol> ActiveConnections = new ConcurrentDictionary<long, LobbyServerProtocol>();
        private static ConcurrentDictionary<long, LobbySessionInfo> ActiveSessions = new ConcurrentDictionary<long, LobbySessionInfo>();
        
        public static LobbyServerPlayerInfo OnPlayerConnect(LobbyServerProtocol client, RegisterGameClientRequest clientRequest)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(clientRequest.AuthInfo.AccountId);

            client.AccountId = account.AccountId;
            client.SessionToken = clientRequest.SessionInfo.SessionToken;  // we are kinda supposed to validate it
            client.UserName = account.UserName;
            client.SelectedGameType = GameType.PvP;
            client.SelectedSubTypeMask = 0;

            LobbyServerPlayerInfo playerInfo = new LobbyServerPlayerInfo
            {
                AccountId = account.AccountId,
                BannerID = account.AccountComponent.SelectedBackgroundBannerID,
                BotCanTaunt = false,
                CharacterInfo = LobbyCharacterInfo.Of(account.CharacterData[account.AccountComponent.LastCharacter]),
                ControllingPlayerId = 0,
                EffectiveClientAccessLevel = ClientAccessLevel.Full,
                EmblemID = account.AccountComponent.SelectedForegroundBannerID,
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

            ActivePlayers.TryAdd(client.AccountId, playerInfo);
            SessionTokenAccountIDCache.TryAdd(client.SessionToken, playerInfo.AccountId);
            ActiveConnections.TryAdd(client.AccountId, client);
            ActiveSessions.TryAdd(client.AccountId, clientRequest.SessionInfo);

            return playerInfo;
        }

        public static void OnPlayerDisconnect(LobbyServerProtocol client)
        {
            ActivePlayers.TryRemove(client.AccountId, out _);
            SessionTokenAccountIDCache.TryRemove(client.SessionToken, out _);
            ActiveConnections.TryRemove(client.AccountId, out _);
            ActiveSessions.TryRemove(client.AccountId, out _);
        }

        public static LobbyServerPlayerInfo GetPlayerInfo(long accountId)
        {
            LobbyServerPlayerInfo playerInfo = null;
            ActivePlayers.TryGetValue(accountId, out playerInfo);

            return playerInfo;
        }

        public static long GetAccountIdOf(long sessionToken)
        {
            if (SessionTokenAccountIDCache.TryGetValue(sessionToken, out long accountId))
            {
                return accountId;
            }

            return 0;
        }

        public static LobbyServerProtocol GetClientConnection(long accountId)
        {
            LobbyServerProtocol clientConnection = null;
            ActiveConnections.TryGetValue(accountId, out clientConnection);
            return clientConnection;
        }

        public static LobbySessionInfo GetSessionInfo(long accountId)
        {
            LobbySessionInfo sessionInfo = null;
            ActiveSessions.TryGetValue(accountId, out sessionInfo);
            return sessionInfo;
        }
    }
}
