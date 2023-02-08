using System;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using EvoS.Framework;
using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;
using log4net;

namespace EvoS.DirectoryServer.Account
{
    public class LoginManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LoginManager));
        private static readonly HashAlgorithm algorithm = SHA256.Create();
        private static readonly Regex usernameRegex = new Regex(@"^[A-Za-z][A-Za-z_\-0-9]{3,}$");
        private static readonly Regex bannedUsernameRegex = new Regex(@"^(?:(?:changeMeToYour)?user(?:name)?)$", RegexOptions.IgnoreCase);
        private static readonly Regex bannedPasswordRegex = new Regex(@"^(?:(?:changeMeToYour)?password)$", RegexOptions.IgnoreCase);

        public const string PasswordIsIncorrect = "Password is incorrect";
        public const string UserDoesNotExist = "User does not exist";
        public const string InvalidUsername = "Invalid username. " +
                        "Please use only latin characters, numbers, underscore and dash, and start with a letter. " +
                        "4 symbols or more.";
        public const string CannotUseThisUsername = "You cannot use this username. Please choose another.";
        public const string CannotUseThisPassword = "You cannot use this password. Please choose another.";
        public const string FailedToCreateAnAccount = "Failed to crate an account";
        public const string UserNotFound = "User not found";
        public const string SteamIdMissing = "Account lacks SteamId. Please use ARLauncher to link your account to Steam.";
        public const string SteamIdZero = "No SteamId was provided. Please use ARLauncher to create an account or to link existing account to Steam.";
        public const string SteamIdAlreadyUsed = "Provided SteamId was already used for another account. Try logging into it instead. You can reset password if you forgot it.";
        public const string SteamWebApiKeyMissing = "Server is not configured to use SteamWebApi";
        public const string AccountWithSuchSteamIdNotFound = "Account with such SteamId was not found";
        public const string UsernameIsAlreadyUsed = "Provided UserName was already used for another account";

        public static long RegisterOrLogin(AuthInfo authInfo)
        {
            LoginDao.LoginEntry entry = DB.Get().LoginDao.Find(authInfo.UserName);
            return (entry != null)
                ? Login(authInfo.UserName, authInfo._Password, entry)
                : CreateAccount(authInfo.UserName, authInfo._Password, steamId: 0);
        }

        public static long CreateAccount(string username, string password, ulong steamId)
        {
            if (!EvosConfiguration.GetAutoRegisterNewUsers())
            {
                log.Info($"Attempt to login as \"{username}\"");
                throw new ArgumentException(UserDoesNotExist);
            }
            if (!usernameRegex.IsMatch(username))
            {
                log.Info($"Attempt to register as \"{username}\"");
                throw new ArgumentException(InvalidUsername);
            }
            if (bannedUsernameRegex.IsMatch(username))
            {
                log.Info($"Attempt to register as \"{username}\"");
                throw new ArgumentException(CannotUseThisUsername);
            }
            if (bannedPasswordRegex.IsMatch(password))
            {
                log.Info($"Attempt to register with a bad password");
                throw new ArgumentException(CannotUseThisPassword);
            }
            var loginDao = DB.Get().LoginDao;
            if (loginDao.Find(username) != null)
            {
                throw new ArgumentException(UsernameIsAlreadyUsed);
            }
            if (EvosConfiguration.SteamApiEnabled)
            {
                if (steamId == 0)
                {
                    log.Info("Won't allow creating account without SteamId provided");
                    throw new ArgumentException(SteamIdZero);
                }
                if (loginDao.FindBySteamId(steamId) != null)
                {
                    log.Info("Won't allow creating account with already used SteamIds");
                    throw new ArgumentException(SteamIdAlreadyUsed);
                }
            }
            long accountId = GenerateAccountId(username);
            for (int i = 0; loginDao.Find(accountId) != null; ++i)
            {
                accountId++;
                if (i >= 100)
                {
                    log.Error($"Failed to register new user {username}");
                    throw new ApplicationException(FailedToCreateAnAccount);
                }
            }
            string hash = Hash(password);
            loginDao.Save(new LoginDao.LoginEntry
            {
                AccountId = accountId,
                Hash = hash,
                Username = username,
                SteamId = steamId,
            });
            log.Info($"Successfully registered new user {accountId}/{username}");
            return accountId;
        }

        public static long Login(AuthInfo authInfo) => Login(authInfo.UserName, authInfo._Password);
        public static long Login(string username, string password) => Login(username, password, DB.Get().LoginDao.Find(username));

        private static long Login(string username, string password, LoginDao.LoginEntry entry, bool ignoreSteam = false)
        {
            string hash = Hash(password);

            if (entry == null)
            {
                log.Warn($"Attempt to log is as non-existing user {username}");
                throw new ArgumentException(UserNotFound);
            }
            
            if (!entry.Hash.Equals(hash))
            {
                log.Warn($"Failed attempt to log is as {entry.AccountId}/{entry.Username}");
                throw new ArgumentException(PasswordIsIncorrect);
            }

            if (entry.SteamId == 0 && EvosConfiguration.SteamApiEnabled && !ignoreSteam)
            {
                log.Warn("Won't allow logging in for account without SteamId");
                throw new ArgumentException(SteamIdMissing);
            }            

            log.Info($"User {entry.AccountId}/{entry.Username} successfully logged in");
            return entry.AccountId;
        }

        public static void LinkAccountToSteam(string username, string password, ulong steamId)
        {
            if (!EvosConfiguration.SteamApiEnabled)
            {
                throw new ArgumentException(SteamWebApiKeyMissing);
            }
            var loginDao = DB.Get().LoginDao;
            var entry = loginDao.Find(username);
            Login(username, password, entry, ignoreSteam: true); //check that credentials are right
            if (loginDao.FindBySteamId(steamId) != null)
            {
                throw new ArgumentException(SteamIdAlreadyUsed);
            }
            if (steamId == 0)
            {
                throw new ArgumentException(SteamIdZero);
            }
            loginDao.UpdateSteamId(entry, steamId);
        }

        public static string RemindUsername(ulong steamId)
        {
            if (!EvosConfiguration.SteamApiEnabled)
            {
                throw new ArgumentException(SteamWebApiKeyMissing);
            }
            if (steamId == 0)
            {
                throw new ArgumentException(SteamIdZero);
            }
            var entry = DB.Get().LoginDao.FindBySteamId(steamId);
            if (entry == null)
            {
                throw new ArgumentException(AccountWithSuchSteamIdNotFound);
            }
            return entry.Username;
        }

        public static void ResetPassword(ulong steamId, string newPassword)
        {
            if (!EvosConfiguration.SteamApiEnabled)
            {
                throw new ArgumentException(SteamWebApiKeyMissing);
            }
            if (steamId == 0)
            {
                throw new ArgumentException(SteamIdZero);
            }
            if (bannedPasswordRegex.IsMatch(newPassword))
            {
                log.Info($"Attempt to reset password with a bad newPassword");
                throw new ArgumentException(CannotUseThisPassword);
            }
            var loginDao = DB.Get().LoginDao;
            var entry = loginDao.FindBySteamId(steamId);
            if (entry == null)
            {
                throw new ArgumentException(AccountWithSuchSteamIdNotFound);
            }
            loginDao.UpdateHash(entry, Hash(newPassword));
        }

        private static long GenerateAccountId(string a)
        {
            int num = (Guid.NewGuid() + a).GetHashCode();
            if (num < 0)
            {
                num = -num;
            }
            return num + 1000000000000000L;
        }

        private static string Hash(string password)
        {
            lock (algorithm)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(EvosConfiguration.GetDBConfig().Salt + password);
                byte[] hashBytes = algorithm.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }
        
        
    }
}