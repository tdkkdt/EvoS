#nullable enable
using System.Collections.Generic;
using System.Linq;
using EvoS.Framework.Auth;
using MongoDB.Bson.Serialization.Attributes;

namespace EvoS.Framework.DataAccess.Daos
{
    public interface LoginDao
    {
        public LoginEntry? Find(string username);
        public LoginEntry? Find(long accountId);
        public List<LoginEntry> FindRegex(string username);
        public LoginEntry? FindByLinkedAccount(LinkedAccount account);
        public List<LoginEntry> FindAll();
        public void Save(LoginEntry entry);

        public class LoginEntry
        {
            [BsonId]
            public long AccountId;
            public required string Username;
            public required string Salt;
            public required string Hash;
            public List<LinkedAccount> LinkedAccounts = new();

            public LinkedAccount? GetLinkedAccount(LinkedAccount template)
            {
                return LinkedAccounts.FirstOrDefault(template.IsSame);
            }
        }
    }
}