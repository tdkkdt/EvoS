using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EvoS.DirectoryServer.Account;
using EvoS.DirectoryServer.ARLauncher.Messages;
using EvoS.Framework.Network.Static;
using log4net.Core;
using MongoDB.Driver.Core.Authentication;
using static EvoS.DirectoryServer.ARLauncher.Messages.LauncherRequest;

namespace EvoS.DirectoryServer.ARLauncher
{
    internal static class LauncherHandler
    {
        public static async Task<LauncherResponseBase> ProcessRequestAsync(LauncherRequest request)
        {
            return request.RequestType switch
            {
                LauncherRequestType.CreateAccount => await CreateAccountAsync(request),
                LauncherRequestType.LinkExistingAccountToSteam => await LinkExistingAccountToSteamAsync(request),
                LauncherRequestType.LogIn => LogIn(request),
                LauncherRequestType.RemindUsername => await RemindUsernameAsync(request),
                LauncherRequestType.ResetPassword => await ResetPasswordAsync(request),
                _ => throw new Exception("unreachable")
            };
        }

        private static async Task<CreateAccountResponse> CreateAccountAsync(LauncherRequest request)
        {
            if (request.Username == null || request.Password == null)
                return new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.UsernameOrPasswordProhibited);

            var resp = await SteamWebApiConnector.Instance.GetSteamIdAsync(request.SteamTicket);
            var response = resp.ResultCode switch
            {
                SteamWebApiConnector.GetSteamIdResult.SteamTicketInvalid => new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.SteamTicketInvalid),
                SteamWebApiConnector.GetSteamIdResult.SteamServersDown => new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.SteamServersDown),
                _ => null
            };
            if (response != null)
                return response;
            try
            {
                LoginManager.Register(new AuthInfo { UserName = request.Username, Password = request.Password}, resp.SteamId);
                return new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.Success);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    LoginManager.UserDoesNotExist => new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.OtherError, "Autoregistering is disabled"),
                    _ when ex.Message == LoginManager.InvalidUsername || ex.Message == LoginManager.CannotUseThisUsername || ex.Message == LoginManager.CannotUseThisPassword //cause 'or pattern' is not available in C# 8.0
                        => new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.UsernameOrPasswordProhibited, ex.Message),
                    LoginManager.SteamIdZero => new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.SteamTicketInvalid),
                    LoginManager.SteamIdAlreadyUsed => new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.SteamAccountAlreadyUsed),
                    LoginManager.UsernameIsAlreadyUsed => new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.UsernameWasAlreadyUsed),
                    _ => new CreateAccountResponse(CreateAccountResponse.CreateAccountResponseType.OtherError, ex.Message)
                };
            }
        }

        private static async Task<LinkToSteamResponse> LinkExistingAccountToSteamAsync(LauncherRequest request)
        {
            if (request.Username == null || request.Password == null)
            {
                return new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.UsernameOrPasswordInvalid);
            }

            var resp = await SteamWebApiConnector.Instance.GetSteamIdAsync(request.SteamTicket);
            var response = resp.ResultCode switch
            {
                SteamWebApiConnector.GetSteamIdResult.SteamTicketInvalid => new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.SteamTicketInvalid),
                SteamWebApiConnector.GetSteamIdResult.SteamServersDown => new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.SteamServersDown),
                SteamWebApiConnector.GetSteamIdResult.NoSteam => new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.NoSteam),
                _ => null
            };
            if (response != null)
                return response;
            
            try
            {
                LoginManager.LinkAccountToSteam(request.Username, request.Password, resp.SteamId);
                return new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.Success);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    LoginManager.SteamWebApiKeyMissing => new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.NoSteam),
                    LoginManager.SteamIdAlreadyUsed => new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.SteamAccountAlreadyUsed),
                    LoginManager.SteamIdZero => new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.SteamTicketInvalid),
                    _ when ex.Message == LoginManager.UserNotFound || ex.Message == LoginManager.PasswordIsIncorrect
                        => new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.UsernameOrPasswordInvalid, ex.Message),
                    _ => new LinkToSteamResponse(LinkToSteamResponse.LinkToSteamResponseType.OtherError, ex.Message)
                };
            }
        }

        private static LogInResponse LogIn(LauncherRequest request)
        {
            if (request.Username == null || request.Password == null)
            {
                return new LogInResponse(LogInResponse.LogInResponseType.UsernameOrPasswordInvalid);
            }
            try
            {
                LoginManager.Login(request.Username, request.Password);
                return new LogInResponse(LogInResponse.LogInResponseType.Success);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    LoginManager.SteamIdMissing => new LogInResponse(LogInResponse.LogInResponseType.MustLinkExistingAccountToSteam),
                    _ when ex.Message == LoginManager.UserNotFound || ex.Message == LoginManager.PasswordIsIncorrect
                        => new LogInResponse(LogInResponse.LogInResponseType.UsernameOrPasswordInvalid, ex.Message),
                    _ => new LogInResponse(LogInResponse.LogInResponseType.OtherError, ex.Message)
                };
            }
        }

        private static async Task<RemindUsernameResponse> RemindUsernameAsync(LauncherRequest request)
        {
            var resp = await SteamWebApiConnector.Instance.GetSteamIdAsync(request.SteamTicket);
            var response = resp.ResultCode switch
            {
                SteamWebApiConnector.GetSteamIdResult.SteamTicketInvalid => new RemindUsernameResponse(RemindUsernameResponse.RemindUsernameResponseType.SteamTicketInvalid),
                SteamWebApiConnector.GetSteamIdResult.SteamServersDown => new RemindUsernameResponse(RemindUsernameResponse.RemindUsernameResponseType.SteamServersDown),
                SteamWebApiConnector.GetSteamIdResult.NoSteam => new RemindUsernameResponse(RemindUsernameResponse.RemindUsernameResponseType.NoSteam),
                _ => null
            };
            if (response != null)
                return response;
            try
            {
                var username = LoginManager.RemindUsername(resp.SteamId);
                return new RemindUsernameResponse(RemindUsernameResponse.RemindUsernameResponseType.Success)
                {
                    Username = username,
                };
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    LoginManager.SteamWebApiKeyMissing => new RemindUsernameResponse(RemindUsernameResponse.RemindUsernameResponseType.NoSteam),
                    LoginManager.SteamIdZero => new RemindUsernameResponse(RemindUsernameResponse.RemindUsernameResponseType.SteamTicketInvalid),
                    LoginManager.AccountWithSuchSteamIdNotFound => new RemindUsernameResponse(RemindUsernameResponse.RemindUsernameResponseType.SteamAccountNotUsed),
                    _ => new RemindUsernameResponse(RemindUsernameResponse.RemindUsernameResponseType.OtherError, ex.Message)
                };
            }
        }

        private static async Task<ResetPasswordResponse> ResetPasswordAsync(LauncherRequest request)
        {
            if (request.Password == null)
                return new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.PasswordProhibited);

            var resp = await SteamWebApiConnector.Instance.GetSteamIdAsync(request.SteamTicket);
            var response = resp.ResultCode switch
            {
                SteamWebApiConnector.GetSteamIdResult.SteamTicketInvalid => new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.SteamTicketInvalid),
                SteamWebApiConnector.GetSteamIdResult.SteamServersDown => new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.SteamServersDown),
                SteamWebApiConnector.GetSteamIdResult.NoSteam => new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.NoSteam),
                _ => null
            };
            if (response != null)
                return response;
            try
            {
                LoginManager.ResetPassword(resp.SteamId, request.Password);
                return new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.Success);
            }
            catch (Exception ex)
            {
                return ex.Message switch
                {
                    LoginManager.SteamWebApiKeyMissing => new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.NoSteam),
                    LoginManager.SteamIdZero => new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.SteamTicketInvalid),
                    LoginManager.CannotUseThisPassword => new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.PasswordProhibited),
                    LoginManager.AccountWithSuchSteamIdNotFound => new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.AccountNotFound),
                    _ => new ResetPasswordResponse(ResetPasswordResponse.ResetPasswordResponseType.OtherError, ex.Message)
                };
            }
        }
    }
}
