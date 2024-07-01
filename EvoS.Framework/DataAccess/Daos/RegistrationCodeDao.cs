using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace EvoS.Framework.DataAccess.Daos
{
    public interface RegistrationCodeDao
    {
        public const int LIMIT = 25;
        
        public RegistrationCodeEntry Find(string code);
        public List<RegistrationCodeEntry> FindBefore(int limit, DateTime dateTime);
        public List<RegistrationCodeEntry> FindAll(int limit, int offset);
        public void Save(RegistrationCodeEntry entry);

        public class RegistrationCodeEntry
        {
            [BsonId]
            public string Code;
            public long IssuedBy;
            public string IssuedTo;
            public DateTime IssuedAt;
            public DateTime ExpiresAt;
            public DateTime UsedAt;
            public long UsedBy;

            [JsonIgnore]
            public bool IsValid => !IsUsed && !HasExpired;

            [JsonIgnore]
            public bool IsUsed => UsedBy != 0;

            [JsonIgnore]
            public bool HasExpired => ExpiresAt < DateTime.UtcNow;

            public RegistrationCodeEntry Use(long accountId)
            {
                return new RegistrationCodeEntry
                {
                    Code = Code,
                    IssuedBy = IssuedBy,
                    IssuedTo = IssuedTo,
                    IssuedAt = IssuedAt,
                    ExpiresAt = ExpiresAt,
                    UsedAt = DateTime.UtcNow,
                    UsedBy = accountId
                };
            }
        }
    }
}