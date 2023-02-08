using EvoS.Framework;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EvoS.DirectoryServer.ARLauncher
{
    public class SteamWebApiConnector
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(SteamWebApiConnector));

        private static Lazy<SteamWebApiConnector> _instance = new Lazy<SteamWebApiConnector>(() => new SteamWebApiConnector());
        public static SteamWebApiConnector Instance => _instance.Value;

        private HttpClient _httpClient;

        public bool Enabled { get; }

        private readonly string _steamWebApiKey;
        private const string _steamWebApiUrl = "https://api.steampowered.com/";
        private SteamWebApiConnector()
        {
            _steamWebApiKey = EvosConfiguration.GetSteamWebApiKey();
            if (!EvosConfiguration.SteamApiEnabled)
            {
                Enabled = false;
                return;
            }
            Enabled = true;
            _httpClient = new HttpClient();
        }

        public enum GetSteamIdResult
        {
            Success,
            NoSteam, //!SteamApiEnabled
            SteamTicketInvalid,
            SteamServersDown,
        }

        public class Response
        {
            public readonly GetSteamIdResult ResultCode;
            public readonly ulong SteamId;
            public Response(GetSteamIdResult resultCode, ulong steamId)
            {
                ResultCode = resultCode;
                SteamId = steamId;
            }
        }
        
        public async Task<Response> GetSteamIdAsync(string ticket)
        {
            if (!Enabled)
                return new Response(GetSteamIdResult.NoSteam, 0);

            try
            {
                var content = await SendWebRequestAsync("ISteamUserAuth", "AuthenticateUserTicket", 1, new List<(string, string)>
                {
                    ("appid", "480"),
                    ("ticket", ticket)
                });
                var jobj = JsonConvert.DeserializeObject<JObject>(content);
                var response = jobj?["response"];
                if (response?["error"]?["errorcode"]?.Value<int>() == 3) //Invalid parameter
                    return new Response(GetSteamIdResult.SteamTicketInvalid, 0);
                var steamId = response?["params"]?["steamid"]?.Value<ulong>();
                if (steamId == null) //some other error?
                {
                    _log.Debug($"SteamWebApi responded in a unexpected way: {content}");
                    return new Response(GetSteamIdResult.SteamTicketInvalid, 0);
                }
                return new Response(GetSteamIdResult.Success, steamId.Value);
            }
            catch (Exception ex)
            {
                _log.Debug("SteamWebApi seems to not respond, or to respond incorrectly?..", ex);
                return new Response(GetSteamIdResult.SteamServersDown, 0);
            }
        }

        private async Task<string> SendWebRequestAsync(string interfaceName, string methodName, int methodVersion, List<(string, string)> parameters)
        {
            parameters.Insert(0, ("key", _steamWebApiKey));
            var parametersString = string.Join("&", parameters.Select(x => $"{x.Item1}={WebUtility.UrlEncode(x.Item2)}"));
            string commandUrl = $"{_steamWebApiUrl}/{interfaceName}/{methodName}/v{methodVersion}/?{parametersString}";
            var httpResponse = await _httpClient.GetAsync(commandUrl).ConfigureAwait(false);
            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsStringAsync();
        }
    }
}
