using EvoS.Framework.DataAccess.Daos;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EvoS.Framework.DataAccess.Mongo;

public class ClientErrorReportMongoDao : MongoDao<ObjectId, ClientErrorReportDao.Entry>, ClientErrorReportDao
{
    public ClientErrorReportMongoDao() : base(
        "client_error_reports", 
        new CreateIndexModel<ClientErrorReportDao.Entry>(Builders<ClientErrorReportDao.Entry>.IndexKeys
            .Ascending(entry => entry.StackTraceHash)
            .Descending(entry => entry.Time)), 
        new CreateIndexModel<ClientErrorReportDao.Entry>(Builders<ClientErrorReportDao.Entry>.IndexKeys
            .Ascending(entry => entry.AccountId)
            .Descending(entry => entry.Time)), 
        new CreateIndexModel<ClientErrorReportDao.Entry>(Builders<ClientErrorReportDao.Entry>.IndexKeys
            .Descending(entry => entry.Time)))
    {
    }

    public ClientErrorReportDao.Entry GetEntry(ObjectId key)
    {
        return findById(key);
    }

    public void SaveEntry(ClientErrorReportDao.Entry data)
    {
        insert(data.Key, data);
    }
}