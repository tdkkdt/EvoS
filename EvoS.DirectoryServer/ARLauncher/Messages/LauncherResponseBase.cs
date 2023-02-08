using System;
using System.Collections.Generic;
using System.Text;

namespace EvoS.DirectoryServer.ARLauncher.Messages
{
    [Serializable]
    public abstract class LauncherResponseBase
    {
        public string? ErrorDescription;
        protected LauncherResponseBase(string? errorDescription)
        {
            ErrorDescription = errorDescription;
        }
    }
}
