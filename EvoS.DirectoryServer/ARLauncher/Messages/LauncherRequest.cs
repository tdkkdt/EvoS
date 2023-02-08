using System;
using System.Collections.Generic;
using System.Text;

namespace EvoS.DirectoryServer.ARLauncher.Messages
{
    [Serializable]
    public class LauncherRequest
    {
        public enum LauncherRequestType
        {
            LogIn,
            CreateAccount,
            LinkExistingAccountToSteam, //only for old accounts that were created before steam integration was coded
            ResetPassword,
            RemindUsername
        }
        public static string NameofKeyField => nameof(IsArLauncherRequest);
        public readonly bool IsArLauncherRequest = true;

        public LauncherRequestType RequestType;

        public string? Username;
        public string? Password;
        public string? SteamTicket;
    }
}
