using System.IO;
using YamlDotNet.Serialization;

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
        
        public bool PingOnGroupRequest = true;

        private static Lazy<EvosConfiguration> _instance = new Lazy<EvosConfiguration>(() =>
            new DeserializerBuilder().Build().Deserialize<EvosConfiguration>(File.ReadAllText("settings.yaml")));

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

        public static bool GetPingOnGroupRequest()
        {
            return GetInstance().PingOnGroupRequest;
        }

        public enum DBType
        {
            None,
            Mongo,
        }

        public class DBConfig
        {
            public DBType Type = DBType.None;
            public string URI = "localhost:27017";
            public string User = "user";
            public string Password = "password";
            public string Database = "atlas";
            public string Salt = "salt";
            public bool MongoDbSrv = false;
        }
    }
}
