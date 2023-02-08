using System;
using System.Collections.Generic;
using System.Text;
using static EvoS.DirectoryServer.ARLauncher.Messages.CreateAccountResponse;

namespace EvoS.DirectoryServer.ARLauncher.Messages
{
    public class LinkToSteamResponse : LauncherResponseBase
    {
        public enum LinkToSteamResponseType
        {
            Success,
            SteamTicketInvalid,
            SteamAccountAlreadyUsed,
            SteamServersDown,
            UsernameOrPasswordInvalid,
            NoSteam,
            OtherError
        }
        public LinkToSteamResponseType ResponseType;
        public LinkToSteamResponse(LinkToSteamResponseType responseType, string errorDescription = null) : base(errorDescription)
        {
            ResponseType = responseType;
        }
    }
}
