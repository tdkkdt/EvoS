using System;
using System.IO;
using System.Threading;
using EvoS.Framework.DataAccess;
using log4net.Config;
using McMaster.Extensions.CommandLineUtils;

namespace EvoS.Sandbox
{
    class Program
    {
        public static int Main(string[] args)
        {
            XmlConfigurator.Configure(new FileInfo("log4net.xml"));
            CommandLineApplication.Execute<Program>(args);
            return 0;
        }
        
        private async void OnExecute()
        {
            Banner.PrintBanner();
            DB.Get();

            // Start Central Server
            await CentralServer.CentralServer.Init(new string[] { });

            // Start Directory Server
            new Thread(StartDirServer).Start();

            CentralServer.CentralServer.MainLoop();
        }

        /// <summary>
        /// Starts the directory server, which is like an authenticator server/lobby load balancer
        /// but in our case it is in charge of giving the ip to the lobby server
        /// </summary>
        static void StartDirServer()
        {
            DirectoryServer.Program.Main();
        }
    }
}
