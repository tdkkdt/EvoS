#nullable enable
using System.Collections.Generic;
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
        public void Save(LoginEntry entry);

        public class LoginEntry
        {
            [BsonId]
            public long AccountId;
            public string Username;
            public string Salt;
            public string Hash;
            public List<LinkedAccount> LinkedAccounts;
        }
    }
}