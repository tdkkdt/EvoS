using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization.NamingConventions;

namespace EvoS.Framework
{
    public class EvosConfiguration
    {
        public int DirectoryServerPort = 6050;
        public string LobbyServerAddress = "127.0.0.1";
        public int LobbyServerPort = 6060;
        public string GameServerExecutable = "";
        public string GameServerExecutableArgs = "";
        public string SteamWebApiKey = "";
        public bool AutoRegisterNewUsers = true;
        public DBConfig Database = new DBConfig();

        private static Lazy<EvosConfiguration> _instance = new Lazy<EvosConfiguration>(() =>
            new YamlDotNet.Serialization.DeserializerBuilder().Build().Deserialize<EvosConfiguration>(File.ReadAllText("settings.yaml")));

        private static EvosConfiguration Instance => _instance.Value;

        public static int GetDirectoryServerPort() => Instance.DirectoryServerPort;

        public static string GetLobbyServerAddress() => Instance.LobbyServerAddress;

        public static int GetLobbyServerPort() => Instance.LobbyServerPort;

        /// <summary>
        /// Full path to server's "AtlasReactor.exe"
        /// </summary>
        /// <returns></returns>
        public static string GetGameServerExecutable() => Instance.GameServerExecutable;

        public static string GetGameServerExecutableArgs() => Instance.GameServerExecutableArgs;

        /// <summary>
        /// You can get one from https://steamcommunity.com/dev/registerkey
        /// </summary>
        /// <returns></returns>
        public static string GetSteamWebApiKey() => Instance.SteamWebApiKey;
        public static bool SteamApiEnabled => !string.IsNullOrWhiteSpace(GetSteamWebApiKey());

        public static bool GetAutoRegisterNewUsers() => Instance.AutoRegisterNewUsers;
        
        public static DBConfig GetDBConfig() => Instance.Database;

        public enum DBType
        {
            None,
            Mongo,
        }

        public class DBConfig
        {
            public DBType Type = DBType.None;
            public string URI = "localhost";
            public string User = "user";
            public string Password = "password";
            public string Database = "atlas";
            public string Salt = "salt";
        }
    }
}
