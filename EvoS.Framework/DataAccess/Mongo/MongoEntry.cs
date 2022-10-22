namespace EvoS.Framework.DataAccess.Mongo
{
    public class MongoEntry<TKey, TEntry>
    {
        public readonly TKey _id;
        public readonly TEntry entry;

        public MongoEntry(TKey id, TEntry entry)
        {
            _id = id;
            this.entry = entry;
        }
    }
}