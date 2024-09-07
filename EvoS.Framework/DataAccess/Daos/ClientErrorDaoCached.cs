using System.Collections.Concurrent;

namespace EvoS.Framework.DataAccess.Daos;

public class ClientErrorDaoCached: ClientErrorDao
{
    private readonly ClientErrorDao dao;
    private readonly ConcurrentDictionary<long, ClientErrorDao.Entry> cache = new();

    public ClientErrorDaoCached(ClientErrorDao dao)
    {
        this.dao = dao;
    }

    private void Cache(ClientErrorDao.Entry entry)
    {
        cache.AddOrUpdate(entry.Key, entry, (k, v) => entry);
    }

    public ClientErrorDao.Entry GetEntry(uint key)
    {
        if (cache.TryGetValue(key, out var entry))
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