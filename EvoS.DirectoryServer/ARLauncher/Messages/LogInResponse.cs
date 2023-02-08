using System;
using System.Collections.Generic;
using System.Text;
using static EvoS.DirectoryServer.ARLauncher.Messages.CreateAccountResponse;

namespace EvoS.DirectoryServer.ARLauncher.Messages
{
    public class LogInResponse : LauncherResponseBase
    {
        public enum LogInResponseType
        {
            Success,
            MustLinkExistingAccountToSteam,
            UsernameOrPasswordInvalid,
            OtherError
        }
        public LogInResponseType ResponseType;
        public LogInResponse(LogInResponseType responseType, string errorDescription = null) : base(errorDescription)
        {
            ResponseType = responseType;
        }
    }
}
