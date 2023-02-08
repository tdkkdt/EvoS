using System;
using System.Collections.Generic;
using System.Text;
using static EvoS.DirectoryServer.ARLauncher.Messages.CreateAccountResponse;

namespace EvoS.DirectoryServer.ARLauncher.Messages
{
    public class ResetPasswordResponse : LauncherResponseBase
    {
        public enum ResetPasswordResponseType
        {
            Success,
            PasswordProhibited,
            SteamTicketInvalid,
            SteamServersDown,
            AccountNotFound,
            NoSteam,
            OtherError
        }
        public ResetPasswordResponseType ResponseType;

        public ResetPasswordResponse(ResetPasswordResponseType responseType, string? errorDescription = null) : base(errorDescription)
        {
            ResponseType = responseType;
        }
    }
}
