#nullable enable
using MongoDB.Bson.Serialization.Attributes;

namespace EvoS.Framework.DataAccess.Daos
{
    public interface LoginDao
    {
        public LoginEntry? Find(string username);
        public LoginEntry? Find(long accountId);
        public LoginEntry? FindBySteamId(ulong steamId);
        public void Save(LoginEntry entry);
        public void UpdateSteamId(LoginEntry entry, ulong newSteamId);
        public void UpdateHash(LoginEntry entry, string hash);

        public class LoginEntry
        {
            [BsonId]
            public long AccountId;
            public string Username;
            public string Hash;
            public ulong SteamId;
        }
    }
}