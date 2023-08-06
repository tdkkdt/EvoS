using System;
using System.Collections.Generic;
using System.Linq;

namespace EvoS.Framework.Auth;

public class LinkedAccount
{
    public AccountType Type { get; private set; }
    public string Id { get; private set; }
    public string DisplayName { get; private set; }
    public uint Level { get; private set; }
    public DateTime CheckTime { get; private set; }
    public bool Active { get; set; }

    public LinkedAccount(AccountType type, string id, string displayName, uint level, DateTime checkTime, bool active)
    {
        this.Type = type;
        this.Id = id;
        this.DisplayName = displayName;
        this.Level = level;
        this.CheckTime = checkTime;
        this.Active = active;
    }

    public LinkedAccount WithLevel(string displayName, uint newLevel, DateTime newCheckTime)
    {
        return new LinkedAccount(Type, Id, displayName, newLevel, newCheckTime, Active);
    }

    public bool IsSame(LinkedAccount other)
    {
        return Type == other.Type && Id.Equals(other.Id);
    }

    public enum AccountType
    {
        UNKNOWN,
        STEAM,
        // DISCORD,
    }

    public class Ticket
    {
        public readonly AccountType Type;
        public readonly string Token;

        public Ticket(AccountType type, string token)
        {
            this.Type = type;
            this.Token = token;
        }
    }

    public class Condition
    {
        public readonly AccountType Type;
        public readonly uint Level;

        public Condition(AccountType type, uint level)
        {
            this.Type = type;
            this.Level = level;
        }

        public bool Matches(List<LinkedAccount> linkedAccounts, bool ignoreLevel = false)
        {
            return linkedAccounts.Any(acc => acc.Type == Type && (acc.Level >= Level || ignoreLevel));
        }

        public override string ToString()
        {
            return $"{Type}";
        }
    }
}