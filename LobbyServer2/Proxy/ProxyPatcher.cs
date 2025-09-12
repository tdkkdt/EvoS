using System;
using System.Collections.Generic;
using CentralServer.BridgeServer;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.WebSocket;
using log4net;

namespace CentralServer.Proxy;

public class ProxyPatcher
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ProxyPatcher));

    public static Dictionary<Type, Func<WebSocketMessage, ProxyConfiguration.Proxy, WebSocketMessage>> Mapping = new()
    {
        { typeof(GameInfoNotification), PatchGameInfoNotification },
        { typeof(GameAssignmentNotification), PatchGameAssignmentNotification },
        { typeof(PreviousGameInfoResponse), PatchPreviousGameInfoResponse },
    };

    public static WebSocketMessage PatchGameInfoNotification(WebSocketMessage message, ProxyConfiguration.Proxy proxy)
    {
        if (message is not GameInfoNotification notify)
        {
            log.Error($"PatchGameInfoNotification received {message.GetType()}");
            return message;
        }

        if (notify.GameInfo is null)
        {
            return message;
        }

        var gameServerAddress = notify.GameInfo.GameServerAddress;
        var addressOverride = GetProxyAddressOverride(gameServerAddress, proxy);

        if (addressOverride is null)
        {
            return notify;
        }

        var newNotify = notify.Clone();
        var newGameInfo = notify.GameInfo.Clone();
        newGameInfo.GameServerAddress = addressOverride;
        log.Debug($"PatchGameInfoNotification {gameServerAddress} -> {addressOverride}");
        newNotify.GameInfo = newGameInfo;
        return newNotify;
    }

    public static WebSocketMessage PatchGameAssignmentNotification(
        WebSocketMessage message,
        ProxyConfiguration.Proxy proxy)
    {
        if (message is not GameAssignmentNotification notify)
        {
            log.Error($"PatchGameAssignmentNotification received {message.GetType()}");
            return message;
        }

        if (notify.GameInfo is null)
        {
            return message;
        }

        var gameServerAddress = notify.GameInfo.GameServerAddress;
        var addressOverride = GetProxyAddressOverride(gameServerAddress, proxy);

        if (addressOverride is null)
        {
            return notify;
        }

        var newNotify = notify.Clone();
        var newGameInfo = notify.GameInfo.Clone();
        newGameInfo.GameServerAddress = addressOverride;
        log.Debug($"PatchGameAssignmentNotification {gameServerAddress} -> {addressOverride}");
        newNotify.GameInfo = newGameInfo;
        return newNotify;
    }

    public static WebSocketMessage PatchPreviousGameInfoResponse(
        WebSocketMessage message,
        ProxyConfiguration.Proxy proxy)
    {
        if (message is not PreviousGameInfoResponse notify)
        {
            log.Error($"PatchPreviousGameInfoResponse received {message.GetType()}");
            return message;
        }

        if (notify.PreviousGameInfo is null)
        {
            return message;
        }

        var gameServerAddress = notify.PreviousGameInfo.GameServerAddress;
        var addressOverride = GetProxyAddressOverride(gameServerAddress, proxy);

        if (addressOverride is null)
        {
            return notify;
        }

        var newNotify = notify.Clone();
        var newGameInfo = notify.PreviousGameInfo.Clone();
        newGameInfo.GameServerAddress = addressOverride;
        log.Debug($"PatchPreviousGameInfoResponse {gameServerAddress} -> {addressOverride}");
        newNotify.PreviousGameInfo = newGameInfo;
        return newNotify;
    }

    private static string GetProxyAddressOverride(string originalAddress, ProxyConfiguration.Proxy proxy)
    {
        if (originalAddress is null)
        {
            return null;
        }
        
        proxy.AddressMapping.TryGetValue(originalAddress, out var addressOverride);
        if (addressOverride is null)
        {
            var server = ServerManager.FindServerByAddress(originalAddress);
            if (server is not null)
            {
                var serverName = server.Name;
                proxy.ServerNameMapping.TryGetValue(serverName, out addressOverride);
            }
        }

        return addressOverride;
    }
}