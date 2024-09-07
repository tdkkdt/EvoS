using LobbyGameClientMessages;
using MongoDB.Bson.Serialization.Attributes;

namespace EvoS.Framework.DataAccess.Daos;

public interface ClientErrorDao
{
    Entry GetEntry(uint Key);
    void SaveEntry(Entry entry);

    public class Entry
    {
        [BsonId] public required long _id;
        public required string LogString;
        public required string StackTrace;
        
        public static Entry Of(ClientErrorReport report)
        {
            return new Entry
            {
                _id = report.StackTraceHash,
                LogString = report.LogString,
                StackTrace = report.StackTrace,
            };
        }

        public long Key => _id;
    }
}