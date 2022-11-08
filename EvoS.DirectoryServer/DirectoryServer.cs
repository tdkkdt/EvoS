using System;
using System.IO;
using System.Net;
using EvoS.DirectoryServer.Account;
using EvoS.Framework;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using log4net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace EvoS.DirectoryServer
{
    public class Program
    {
        public static void Main(string[] args = null)
        {
            var host = WebHost.CreateDefaultBuilder()
                .SuppressStatusMessages(true)
                .UseKestrel(koptions => koptions.Listen(IPAddress.Parse("0.0.0.0"), EvosConfiguration.GetDirectoryServerPort()))
                .UseStartup<DirectoryServer>()
                .Build();

            Console.CancelKeyPress += async (sender, @event) =>
            {
                await host.StopAsync();
                host.Dispose();
            };

            host.Run();
        }
    }

    public class DirectoryServer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DirectoryServer));
        
        public void Configure(IApplicationBuilder app)
        {
            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            log.Info("Started DirectoryServer on '0.0.0.0:6050'");

            app.Run((context) =>
            {
                context.Response.ContentType = "application/json";
                MemoryStream ms = new MemoryStream();
                context.Request.Body.CopyTo(ms);
                ms.Position = 0;
                string requestBody = new StreamReader(ms).ReadToEnd(); ;
                ms.Dispose();

                AssignGameClientRequest request = JsonConvert.DeserializeObject<AssignGameClientRequest>(requestBody);
                AssignGameClientResponse response = new AssignGameClientResponse
                {
                    RequestId = request.RequestId,
                    ResponseId = request.ResponseId,
                    Success = true,
                    ErrorMessage = ""
                };

                PersistedAccountData account;
                try
                {
                    account = DB.Get().AccountDao.GetAccount(request.AuthInfo.AccountId);
                    if (account == null)
                    {
                        log.Info($"Player {request.AuthInfo.AccountId} doesnt exists");
                        DB.Get().AccountDao.CreateAccount(NewAccount(request));
                        account = DB.Get().AccountDao.GetAccount(request.AuthInfo.AccountId);
                        if (account != null)
                        {
                            log.Info($"Successfully Registered {account.Handle}/{account.AccountId}");
                        }
                        else
                        {
                            log.Error($"Error creating a new account for player '{request.AuthInfo.UserName}'/{request.AuthInfo.AccountId}");
                            account = NewAccount(request);
                            log.Error($"Temp user {account.Handle}/{account.AccountId}");
                        }
                    }
                    else
                    {
                        log.Info($"Player {account.Handle}/{request.AuthInfo.AccountId} logged in");
                    }
                }
                catch (Exception e)
                {
                    account = NewAccount(request);
                    log.Error($"Temp user {account.Handle}/{account.AccountId}: {e}");
                }

                request.SessionInfo.SessionToken = 0;

                response.SessionInfo = request.SessionInfo;
                response.SessionInfo.AccountId = account.AccountId;
                response.SessionInfo.Handle = account.UserName;
                response.SessionInfo.ConnectionAddress = "127.0.0.1";
                response.SessionInfo.ProcessCode = "";
                response.SessionInfo.FakeEntitlements = "";
                response.SessionInfo.LanguageCode = "EN"; // Needs to be uppercase

                response.LobbyServerAddress = EvosConfiguration.GetLobbyServerAddress();

                LobbyGameClientProxyInfo proxyInfo = new LobbyGameClientProxyInfo
                {
                    AccountId = response.SessionInfo.AccountId,
                    SessionToken = request.SessionInfo.SessionToken,
                    AssignmentTime = 1565574095,
                    Handle = request.SessionInfo.Handle,
                    Status = ClientProxyStatus.Assigned
                };

                response.ProxyInfo = proxyInfo;

                return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            });
        }

        private static PersistedAccountData NewAccount(AssignGameClientRequest request)
        {
            return AccountManager.CreateAccount(request);
        }
    }
}
