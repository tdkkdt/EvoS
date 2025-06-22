using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using log4net;
using YamlDotNet.Serialization;

namespace CentralServer.Proxy;

public class ProxyConfiguration
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ProxyConfiguration));
    private static ProxyConfiguration Instance = null;

    public List<Proxy> Proxies = new();

    [NonSerialized]
    public Dictionary<IPAddress, Proxy> ProxyByAddress = new();

    private static ProxyConfiguration GetInstance()
    {
        if (Instance == null)
        {
            Load();
        }

        return Instance;
    }

    public static bool Invalidate()
    {
        return Load();
    }

    private static bool Load()
    {
        var deserializer = new DeserializerBuilder().Build();

        try
        {
            Instance = deserializer.Deserialize<ProxyConfiguration>(File.ReadAllText("Config/proxy.yaml"));

            Instance.ProxyByAddress = Instance.Proxies.ToDictionary(proxy => IPAddress.Parse(proxy.ProxyAddress));

            log.Info(
                $"Configured proxies: {(Instance.ProxyByAddress.Count > 0
                    ? string.Join(", ", Instance.ProxyByAddress.Keys.Select(p => p.ToString()))
                    : "none")}");
            return true;
        }
        catch (Exception e)
        {
            log.Error("Failed to read proxy config", e);
        }

        return false;
    }

    public static Dictionary<IPAddress, Proxy> GetProxies() => GetInstance()?.ProxyByAddress;

    public class Proxy
    {
        public string Name;
        public string ProxyAddress;
        public string LobbyAddress;
        public Dictionary<string, string> AddressMapping = new();
        public Dictionary<string, string> ServerNameMapping = new();

        public string GetName() => Name ?? ProxyAddress;
    }
}