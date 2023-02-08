using System;
using System.Collections.Generic;
using System.Text;

namespace EvoS.DirectoryServer.ARLauncher.Messages
{
    public class CreateAccountResponse : LauncherResponseBase
    {
        public enum CreateAccountResponseType
        {
            Success,
            UsernameWasAlreadyUsed,
            UsernameOrPasswordProhibited,
            SteamTicketInvalid,
            SteamAccountAlreadyUsed,
            SteamServersDown,
            OtherError
        }
        public CreateAccountResponseType ResponseType;
        public CreateAccountResponse(CreateAccountResponseType responseType, string? errorDescription = null) : base(errorDescription)
        {
            ResponseType = responseType;
        }
    }
}
