using EvoS.Framework.DataAccess.Daos;

namespace EvoS.Framework.DataAccess.Mongo;

public class ClientErrorMongoDao : MongoDao<long, ClientErrorDao.Entry>, ClientErrorDao
{
    public ClientErrorMongoDao() : base("client_errors")
    {
    }

    public ClientErrorDao.Entry GetEntry(uint key)
    {
        return findById(key);
    }

    public void SaveEntry(ClientErrorDao.Entry data)
    {
        insert(data.Key, data);
    }
}