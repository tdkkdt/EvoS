using System;
using System.Threading;
using System.Threading.Tasks;
using CentralServer.LobbyServer.Session;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;
using log4net;

namespace CentralServer.LobbyServer
{
    class AdminManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminManager));
        
        private readonly CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        private static AdminManager _instance;

        public static AdminManager Get()
        {
            return _instance ??= new AdminManager();
        }
        
        public void Start()
        {
            _ = UpdateMutedLoop(cancelTokenSource.Token);
        }

        private async Task UpdateMutedLoop(CancellationToken cancelToken)
        {
            while (true)
            {
                if (cancelToken.IsCancellationRequested) return;
                UpdateMuted();
                await Task.Delay(300_000, cancelToken);
            }
        }

        private void UpdateMuted()
        {
            foreach (long accountId in SessionManager.GetOnlinePlayers())
            {
                PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
                if (account == null)
                {
                    log.Warn($"Cannot update muted status: account {accountId} not found");
                    continue;
                }

                if (account.AdminComponent.Muted && account.AdminComponent.MutedUntil <= DateTime.UtcNow)
                {
                    log.Info($"UNMUTE {account.Handle}: time out");
                    account.AdminComponent.Muted = false;
                    DB.Get().AccountDao.UpdateAccount(account);
                }
            }
        }

        public bool Mute(long accountId, TimeSpan duration, string adminUsername, string description)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
            if (account == null)
            {
                log.Error($"Cannot mute: account {accountId} not found");
                return false;
            }
            
            DateTime time = DateTime.UtcNow;
            bool muted = duration.Ticks > 0;
            account.AdminComponent.AdminActions.Add(new AdminComponent.AdminActionRecord
            {
                AdminUsername = adminUsername,
                ActionType = muted ? AdminComponent.AdminActionType.Mute : AdminComponent.AdminActionType.Unmute,
                Duration = duration,
                Description = description,
                Time = time,
            });
            account.AdminComponent.MutedUntil = time + duration;
            account.AdminComponent.Muted = muted;
            DB.Get().AccountDao.UpdateAccount(account);
            
            string logString = muted
                ? $"MUTE {account.Handle} for {duration}"
                : $"UNMUTE {account.Handle}";
            log.Info($"{logString} by {adminUsername}: {description}");
            return true;
        }

        public void UpdateBanned(long accountId)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
            if (account == null)
            {
                log.Warn($"Cannot update banned status: account {accountId} not found");
                return;
            }
            
            if (account.AdminComponent.Locked && account.AdminComponent.LockedUntil <= DateTime.UtcNow)
            {
                log.Info($"UNBAN {account.Handle}: time out");
                account.AdminComponent.Locked = false;
                DB.Get().AccountDao.UpdateAccount(account);
            }
        }
        
        public bool Ban(long accountId, TimeSpan duration, string adminUsername, string description)
        {
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
            if (account == null)
            {
                log.Error($"Cannot ban: account {accountId} not found");
                return false;
            }
            
            DateTime time = DateTime.UtcNow;
            bool banned = duration.Ticks > 0;
            account.AdminComponent.AdminActions.Add(new AdminComponent.AdminActionRecord
            {
                AdminUsername = adminUsername,
                ActionType = banned ? AdminComponent.AdminActionType.Lock : AdminComponent.AdminActionType.Unlock,
                Duration = duration,
                Description = description,
                Time = time,
            });
            account.AdminComponent.LockedUntil = time + duration;
            account.AdminComponent.Locked = banned;
            DB.Get().AccountDao.UpdateAccount(account);
            
            string logString = banned
                ? $"BAN {account.Handle} for {duration}"
                : $"UNBAN {account.Handle}";
            log.Info($"{logString} by {adminUsername}: {description}");

            LobbyServerProtocol conn = SessionManager.GetClientConnection(accountId);
            if (conn != null)
            {
                log.Info($"KICK {account.Handle} by {adminUsername}: {description}");
                conn.CloseConnection();
            }
            else
            {
                log.Info($"{account.Handle} is currently offline");
            }

            return true;
        }
    }
}
