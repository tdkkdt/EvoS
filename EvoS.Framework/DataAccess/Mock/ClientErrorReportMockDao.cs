using EvoS.Framework.DataAccess.Daos;
using MongoDB.Bson;

namespace EvoS.Framework.DataAccess.Mock;

public class ClientErrorReportMockDao: ClientErrorReportDao
{
    public ClientErrorReportDao.Entry GetEntry(ObjectId key)
    {
        return null;
    }

    public void SaveEntry(ClientErrorReportDao.Entry entry) { }
}