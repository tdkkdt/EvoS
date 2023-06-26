using System;
using System.Collections.Generic;
using System.Net;
using EvoS.Framework.Constants.Enums;

namespace EvoS.Framework.Network.Static
{
    [Serializable]
    [EvosMessage(593)]
    public class AdminComponent
    {
        public AdminComponent()
        {
            AdminActions = new List<AdminActionRecord>();
            ActiveQueuePenalties = new Dictionary<GameType, QueuePenalties>();
            Region = Region.US;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public AdminComponent CloneForClient()
        {
            return new AdminComponent
            {
                GameLeavingPoints = GameLeavingPoints
            };
        }

        public bool Locked { get; private set; }

        public DateTime LockedUntil { get; private set; }

        public bool Muted { get; private set; }

        public DateTime MutedUntil { get; private set; }

        public bool Online { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime LastLogout { get; set; }

        public string LastLogoutSessionToken { get; set; }

        public Region Region { get; set; }

        public string LanguageCode { get; set; }

        [EvosMessage(594)]
        public List<AdminActionRecord> AdminActions { get; set; }

        [EvosMessage(598)]
        public Dictionary<GameType, QueuePenalties> ActiveQueuePenalties { get; set; }

        public float GameLeavingPoints { get; set; }

        public DateTime GameLeavingLastForgivenessCheckpoint { get; set; }

        [NonSerialized] public Dictionary<string, LoginStats> LoginHistory;

        public void RecordLogin(IPAddress ipAddress)
        {
            DateTime time = DateTime.UtcNow;
            LastLogin = time;
            LoginHistory ??= new Dictionary<string, LoginStats>();

            string ip = ipAddress.ToString();
            if (LoginHistory.TryGetValue(ip, out LoginStats stats))
            {
                stats.lastLogin = time;
                stats.totalLogins++;
            }
            else
            {
                LoginHistory.Add(ip, new LoginStats { lastLogin = time, totalLogins = 1 });
            }
        }

        [EvosMessage(597)]
        public enum AdminActionType
        {
            Lock,
            Unlock,
            Mute,
            Unmute,
            Kick,
            Alter
        }

        [Serializable]
        [EvosMessage(596)]
        public class AdminActionRecord
        {
            public string AdminUsername { get; set; }

            public AdminActionType ActionType { get; set; }

            public TimeSpan Duration { get; set; }

            public string Description { get; set; }

            public DateTime Time { get; set; }
        }

        public void Mute(TimeSpan duration, string adminUsername, string description)
        {
            DateTime time = DateTime.UtcNow;
            AdminActions.Add(new AdminActionRecord
            {
                AdminUsername = adminUsername,
                ActionType = AdminActionType.Mute,
                Duration = duration,
                Description = description,
                Time = time,
            });
            MutedUntil = time + duration;
            Muted = true;
        }

        public class LoginStats
        {
            public int totalLogins;
            public DateTime lastLogin;
        }
    }
}
