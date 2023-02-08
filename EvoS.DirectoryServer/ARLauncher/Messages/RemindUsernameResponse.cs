using System;
using System.Collections.Generic;
using System.Text;
using static EvoS.DirectoryServer.ARLauncher.Messages.CreateAccountResponse;

namespace EvoS.DirectoryServer.ARLauncher.Messages
{
    public class RemindUsernameResponse : LauncherResponseBase
    {
        public enum RemindUsernameResponseType
        {
            Success,
            SteamTicketInvalid,
            SteamAccountNotUsed,
            SteamServersDown,
            NoSteam,
            OtherError
        }
        public RemindUsernameResponseType ResponseType;
        public string? Username;

        public RemindUsernameResponse(RemindUsernameResponseType responseType, string? errorDescription = null) : base(errorDescription)
        {
            ResponseType = responseType;
        }
    }
}
