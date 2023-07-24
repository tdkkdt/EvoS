using System.Collections.Generic;
using System.Linq;

namespace EvoS.Framework.Auth;

public class LinkedAccount
{
    public readonly Type type;
    public readonly string id;
    public readonly uint level;

    public LinkedAccount(Type type, string id, uint level)
    {
        this.type = type;
        this.id = id;
        this.level = level;
    }

    public enum Type
    {
        UNKNOWN,
        STEAM,
        // DISCORD,
    }

    public class Ticket
    {
        public readonly Type type;
        public readonly string token;

        public Ticket(Type type, string token)
        {
            this.type = type;
            this.token = token;
        }
    }

    public class Condition
    {
        public readonly Type type;
        public readonly uint level;

        public Condition(Type type, uint level)
        {
            this.type = type;
            this.level = level;
        }

        public bool Matches(List<LinkedAccount> linkedAccounts, bool ignoreLevel = false)
        {
            return linkedAccounts.Any(acc => acc.type == type && (acc.level >= level || ignoreLevel));
        }

        public override string ToString()
        {
            return $"{type}";
        }
    }
}