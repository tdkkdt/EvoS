using BitFaster.Caching.Lru;

namespace EvoS.Framework.DataAccess.Daos;

public class ClientErrorDaoCached: ClientErrorDao
{
    private readonly ClientErrorDao dao;
    
    private const int Capacity = 2048;
    private readonly FastConcurrentLru<long, ClientErrorDao.Entry> cache = new(Capacity);

    public ClientErrorDaoCached(ClientErrorDao dao)
    {
        this.dao = dao;
    }

    private void Cache(ClientErrorDao.Entry entry)
    {
        cache.AddOrUpdate(entry.Key, entry);
    }

    public ClientErrorDao.Entry GetEntry(uint key)
    {
        if (cache.TryGet(key, out var entry))
        {
            return entry;
        }

        var nonCachedEntry = dao.GetEntry(key);
        if (nonCachedEntry != null)
        {
            Cache(nonCachedEntry);
        }
        return nonCachedEntry;
    }

    public void SaveEntry(ClientErrorDao.Entry entry)
    {
        dao.SaveEntry(entry);
        Cache(entry);
    }
}