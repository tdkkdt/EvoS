using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EvoS.DirectoryServer.ARLauncher;
using EvoS.Framework;
using EvoS.Framework.Auth;
using EvoS.Framework.DataAccess;
using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Misc;
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
        public const string AccountWithSuchLinkedAccountNotFound = "Account linked to this third-party account was not found";
        public const string UsernameIsAlreadyUsed = "This username is already in use. Please choose another.";

        public static long RegisterOrLogin(AuthInfo authInfo)
        {
            LoginDao.LoginEntry entry = DB.Get().LoginDao.Find(authInfo.UserName.ToLower());
            return entry != null
                ? Login(authInfo.UserName, authInfo._Password)
                : AutoRegister(authInfo);
        }

        private static long AutoRegister(AuthInfo authInfo)
        {
            if (!EvosConfiguration.GetAutoRegisterNewUsers())
            {
                log.Info($"Attempt to login as \"{authInfo.UserName}\"");
                throw new ArgumentException("User does not exist");
            }

            log.Info($"Registering user automatically: {authInfo.UserName}");
            return Register(authInfo);
        }

        public static long Register(AuthInfo authInfo, List<LinkedAccount.Ticket> linkedAccountTickets = null)
        {
            LoginDao loginDao = DB.Get().LoginDao;
            LoginDao.LoginEntry entry = loginDao.Find(authInfo.UserName.ToLower());

            if (entry is not null)
            {
                log.Info($"Attempt to register as existing user \"{authInfo.UserName}\"");
                throw new ArgumentException(UsernameIsAlreadyUsed);
            }
            
            if (!usernameRegex.IsMatch(authInfo.UserName))
            {
                log.Info($"Attempt to register as \"{authInfo.UserName}\"");
                throw new ArgumentException(InvalidUsername);
            }

            if (bannedUsernameRegex.IsMatch(authInfo.UserName))
            {
                log.Info($"Attempt to register as \"{authInfo.UserName}\"");
                throw new ArgumentException(CannotUseThisUsername);
            }

            if (bannedPasswordRegex.IsMatch(authInfo.Password))
            {
                log.Info($"Attempt to register with a bad password");
                throw new ArgumentException(CannotUseThisPassword);
            }

            List<LinkedAccount> linkedAccounts = ProcessLinkedAccountTickets(linkedAccountTickets);
            ValidateLinkedAccountConditions(EvosConfiguration.GetLinkedAccountRegistrationConditions(), linkedAccounts);

            long accountId = GenerateAccountId(authInfo.UserName);
            for (int i = 0; loginDao.Find(accountId) != null; ++i)
            {
                accountId++;
                if (i >= 100)
                {
                    log.Error($"Failed to register new user {authInfo.UserName}");
                    throw new ApplicationException("Failed to create an account");
                }
            }

            PersistedAccountData account = CreateAccount(accountId, authInfo.UserName);
            if (account is null)
            {
                throw new ApplicationException("Failed to create an account");
            }
            
            SaveLogin(accountId, authInfo.UserName, authInfo._Password, linkedAccounts);
            log.Info($"Successfully registered new user {accountId}/{authInfo.UserName}");
            return accountId;
        }

        private static List<LinkedAccount> ProcessLinkedAccountTickets(List<LinkedAccount.Ticket> linkedAccountTickets)
        {
            if (linkedAccountTickets is null)
            {
                return new List<LinkedAccount>();
            }
            
            List<LinkedAccount> linkedAccounts = linkedAccountTickets.Select(CheckLinkedAccountTicket).ToList();
            foreach (LinkedAccount linkedAccount in linkedAccounts)
            {
                LoginDao.LoginEntry existingAccount = DB.Get().LoginDao.FindByLinkedAccount(linkedAccount);
                if (existingAccount != null)
                {
                    log.Info(
                        $"Won't allow creating account with {linkedAccount.type} already linked to {existingAccount.Username}/{existingAccount.AccountId}");
                    throw new ArgumentException(
                        $"This {linkedAccount.type} account is already linked to an existing Atlas Reactor account. Try logging into it instead.");
                }
            }
            return linkedAccounts;
        }

        private static void ValidateLinkedAccountConditions(List<List<LinkedAccount.Condition>> conditions, List<LinkedAccount> linkedAccounts)
        {
            foreach (List<LinkedAccount.Condition> condition in conditions)
            {
                if (!condition.Any(c => c.Matches(linkedAccounts)))
                {
                    if (!condition.Any(c => c.Matches(linkedAccounts, true)))
                    {
                        throw new ArgumentException(
                            $"You need to link one of the following third-party accounts: {string.Join(" or ", condition)}");
                    }
                    else
                    {
                        throw new ArgumentException(
                            $"Unfortunately, provided third-party accounts do not match the required trust level. Please, try linking other accounts, or contact support.");
                    }
                }
            }
        }

        private static LinkedAccount CheckLinkedAccountTicket(LinkedAccount.Ticket ticket)
        {
            switch (ticket.type)
            {
                case LinkedAccount.Type.STEAM:
                    SteamWebApiConnector.Response steamResponse = Task.Run(() => SteamWebApiConnector.Instance.GetSteamIdAsync(ticket.token)).GetAwaiter().GetResult();
                    if (steamResponse.ResultCode != SteamWebApiConnector.GetSteamIdResult.Success || steamResponse.SteamId == 0UL)
                    {
                        throw new EvosException("Failed to verify Steam account"); // TODO more specific errors
                    }

                    return new LinkedAccount(
                        LinkedAccount.Type.STEAM,
                        steamResponse.SteamId.ToString(),
                        0);
                default:
                    throw new EvosException($"{ticket.type} account type is not supported");
            }
        }

        public static PersistedAccountData CreateAccount(long accountId, string username)
        {
            DB.Get().AccountDao.CreateAccount(AccountManager.CreateAccount(accountId, username));
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
            if (account == null)
            {
                log.Error($"Error creating a new account for player '{username}'/{accountId}");
            }

            return account;
        }

        private static void SaveLogin(long accountId, string username, string password, List<LinkedAccount> linkedAccounts)
        {
            LoginDao loginDao = DB.Get().LoginDao;
            string salt = GenerateSalt();
            string hash = Hash(salt, password);
            loginDao.Save(new LoginDao.LoginEntry
            {
                AccountId = accountId,
                Salt = salt,
                Hash = hash,
                Username = username.ToLower(),
                LinkedAccounts = linkedAccounts,
            });
            log.Info($"Successfully generated new password hash for {accountId}/{username}");
        }

        public static long Login(string username, string password, bool ignoreSteam = false)
        {
            LoginDao.LoginEntry entry = DB.Get().LoginDao.Find(username);
            if (entry == null)
            {
                log.Warn($"Attempt to log is as non-existing user {username}");
                throw new ArgumentException(UserNotFound);
            }

            string hash = Hash(entry.Salt, password);
            if (!entry.Hash.Equals(hash))
            {
                log.Warn($"Failed attempt to log is as {entry.AccountId}/{entry.Username}");
                throw new ArgumentException(PasswordIsIncorrect);
            }
            
            // TODO update linked account level
            ValidateLinkedAccountConditions(EvosConfiguration.GetLinkedAccountLoginConditions(), entry.LinkedAccounts);
            
            log.Info($"User {entry.AccountId}/{entry.Username} successfully logged in");
            if (entry.Salt.IsNullOrEmpty())
            {
                UpdatePassword(entry, password);
            }
            return entry.AccountId;
        }

        // TODO API
        public static void LinkAccounts(long accountId, List<LinkedAccount.Ticket> tickets)
        {
            List<LinkedAccount> linkedAccounts = ProcessLinkedAccountTickets(tickets);
            
            var loginDao = DB.Get().LoginDao;
            var entry = loginDao.Find(accountId);
            if (entry is null)
            {
                throw new ArgumentException(UserNotFound);
            }
            entry.LinkedAccounts.AddRange(linkedAccounts); // TODO multiple accounts of the same type?
            loginDao.Save(entry);
        }

        // TODO API?
        public static string RemindUsername(LinkedAccount.Ticket ticket)
        {
            LinkedAccount linkedAccount = CheckLinkedAccountTicket(ticket); // TODO which account types are allowed here? (from config)
            var entry = DB.Get().LoginDao.FindByLinkedAccount(linkedAccount);
            if (entry == null)
            {
                throw new ArgumentException(AccountWithSuchLinkedAccountNotFound);
            }
            return entry.Username;
        }

        // TODO API
        public static void ResetPassword(LinkedAccount.Ticket ticket, string newPassword)
        {
            LinkedAccount linkedAccount = CheckLinkedAccountTicket(ticket); // TODO which account types are allowed here? (from config)
            if (bannedPasswordRegex.IsMatch(newPassword))
            {
                log.Info($"Attempt to reset password with a bad newPassword");
                throw new ArgumentException(CannotUseThisPassword);
            }
            var loginDao = DB.Get().LoginDao;
            var entry = loginDao.FindByLinkedAccount(linkedAccount);
            if (entry == null)
            {
                throw new ArgumentException(AccountWithSuchLinkedAccountNotFound);
            }

            UpdatePassword(entry, newPassword);
        }

        private static void UpdatePassword(LoginDao.LoginEntry entry, string newPassword)
        {
            SaveLogin(entry.AccountId, entry.Username, newPassword, entry.LinkedAccounts);
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

        private static string Hash(string customSaltPart, string password)
        {
            lock (algorithm)
            {
                if (customSaltPart.IsNullOrEmpty()) customSaltPart = string.Empty;
                byte[] bytes = Encoding.UTF8.GetBytes(EvosConfiguration.GetDBConfig().Salt + customSaltPart + password);
                byte[] hashBytes = algorithm.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private static string GenerateSalt()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public static string GenerateApiKey()
        {
            return GenerateSalt();
        }

        public static void RevokeActiveTickets(long accountId)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
            if (account is null)
            {
                throw new EvosException("Account not found");
            }

            account.ApiKey = GenerateApiKey();
            DB.Get().AccountDao.UpdateAccount(account);
        }
    }
}