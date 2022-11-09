#nullable enable
using MongoDB.Bson.Serialization.Attributes;

namespace EvoS.Framework.DataAccess.Daos
{
    public interface LoginDao
    {
        public LoginEntry? Find(string username);
        public LoginEntry? Find(long accountId);
        public void Save(LoginEntry entry);

        public class LoginEntry
        {
            [BsonId]
            public long AccountId;
            public string Username;
            public string Hash;
        }
    }
}