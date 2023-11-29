using System;
using System.Collections.Generic;
using EvoS.Framework.Misc;
using log4net;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class MongoDao<TKey, TEntry> where TEntry: class
    {
        private static readonly ILog log = LogManager.GetLogger("MongoDao");
        
        private const MongoCollectionSettings settings = null;

        protected readonly IMongoCollection<TEntry> c;
        
        protected MongoDao(string collectionName, params CreateIndexModel<TEntry>[] indices)
        {
            IMongoDatabase mongo = MongoDB.GetInstance();
            List<string> collections = mongo.ListCollectionNames().ToList();
            bool init = !collections.Contains(collectionName);
            if (init)
            {
                log.Info($"Collection {collectionName} not found in [{string.Join(", ", collections)}], creating...");
                try
                {
                    MongoDB.GetInstance().CreateCollection(collectionName); // TODO MONGO settings
                }
                catch (Exception e)
                {
                    log.Error("Failed to create a collection", e);
                    throw;
                }
            }
            c = mongo.GetCollection<TEntry>(collectionName, settings);
            if (init && !indices.IsNullOrEmpty())
            {
                try
                {
                    c.Indexes.CreateMany(indices);
                }
                catch (Exception e)
                {
                    log.Error("Failed to create an index", e);
                    throw;
                }
            }
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
        protected SortDefinitionBuilder<TEntry> s => Builders<TEntry>.Sort;
        protected ProjectionDefinitionBuilder<TEntry> p => Builders<TEntry>.Projection;

        private FilterDefinition<TEntry> Key(TKey id)
        {
            return f.Eq("_id", id);
        }
    }
}