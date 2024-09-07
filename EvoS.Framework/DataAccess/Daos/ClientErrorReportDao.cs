using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EvoS.Framework.DataAccess.Daos;

public interface ClientErrorReportDao
{
    Entry GetEntry(ObjectId Key);
    void SaveEntry(Entry entry);

    class Entry
    {
        [BsonId] public ObjectId _id = ObjectId.GenerateNewId();
        public long StackTraceHash;
        public long AccountId;
        public DateTime Time;
        public uint Count;
        public string Version;

        public ObjectId Key => _id;
    }
}