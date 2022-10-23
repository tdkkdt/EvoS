using EvoS.Framework.Logging;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class MongoDao<TKey, TEntry> where TEntry: class
    {
        private const MongoCollectionSettings settings = null;

        protected readonly IMongoCollection<TEntry> c;
        
        protected MongoDao(string collectionName)
        {
            c = MongoDB.GetInstance().GetCollection<TEntry>(collectionName, settings);
        }

        protected TEntry findById(TKey id)
        {
            return c.Find(Key(id)).FirstOrDefault();
        }

        protected void insert(TKey id, TEntry entry)
        {
            c.ReplaceOne(
                filter: Key(id),
                options: new ReplaceOptions { IsUpsert = true },
                replacement: entry);
        }
        
        protected FilterDefinitionBuilder<TEntry> f => Builders<TEntry>.Filter;
        protected UpdateDefinitionBuilder<TEntry> u => Builders<TEntry>.Update;

        private FilterDefinition<TEntry> Key(TKey id)
        {
            return f.Eq("_id", id);
        }
    }
}