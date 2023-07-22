using System.IO;
using YamlDotNet.Serialization;
using EvoS.Framework.Misc;

namespace EvoS.Framework
{
    public class EvosConfiguration
    {
        private static EvosConfiguration Instance = null;
        public int DirectoryServerPort = 6050;
        public string LobbyServerAddress = "127.0.0.1";
        public int LobbyServerPort = 6060;
        public string GameServerExecutable = "";
        public string GameServerExecutableArgs = "";
        public bool AutoRegisterNewUsers = true;
        public DBConfig Database = new DBConfig();
        public string UserApiKey = "";
        public int UserApiPort = 3002;
        public string AdminApiKey = "";
        public int AdminApiPort = 3001;
        public string TicketAuthKey = "";
        public bool AllowUsernamePasswordAuth = true;
        
        public bool PingOnGroupRequest = true;

        private static EvosConfiguration GetInstance()
        {
            if (Instance == null)
            {
                var deserializer = new DeserializerBuilder()
                    .Build();

                Instance = deserializer.Deserialize<EvosConfiguration>(File.ReadAllText("settings.yaml"));
            }

            return Instance;
        }

        public static int GetDirectoryServerPort()
        {
            return GetInstance().DirectoryServerPort;
        }

        public static string GetLobbyServerAddress()
        {
            return GetInstance().LobbyServerAddress;
        }

        public static int GetLobbyServerPort()
        {
            return GetInstance().LobbyServerPort;
        }

        /// <summary>
        /// Full path to server's "AtlasReactor.exe"
        /// </summary>
        /// <returns></returns>
        public static string GetGameServerExecutable()
        {
            return GetInstance().GameServerExecutable;
        }
        
        public static string GetGameServerExecutableArgs()
        {
            return GetInstance().GameServerExecutableArgs;
        }

        public static bool GetAutoRegisterNewUsers()
        {
            return GetInstance().AutoRegisterNewUsers;
        }
        
        public static DBConfig GetDBConfig()
        {
            return GetInstance().Database;
        }

        public static bool GetPingOnGroupRequest()
        {
            return GetInstance().PingOnGroupRequest;
        }

        public static string GetAdminApiKey()
        {
            return GetInstance().AdminApiKey;
        }

        public static string GetUserApiKey()
        {
            return GetInstance().UserApiKey;
        }

        public static int GetAdminApiPort()
        {
            return GetInstance().AdminApiPort;
        }

        public static int GetUserApiPort()
        {
            return GetInstance().UserApiPort;
        }

        public static string GetTicketAuthKey()
        {
            return GetInstance().TicketAuthKey;
        }

        public static bool GetAllowUsernamePasswordAuth()
        {
            return GetInstance().AllowUsernamePasswordAuth;
        }

        public static bool GetAllowTicketAuth()
        {
            return !GetUserApiKey().IsNullOrEmpty() && !GetTicketAuthKey().IsNullOrEmpty();
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
            public bool UseCredentials = true;
        }
    }
}
