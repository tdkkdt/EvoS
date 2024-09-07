using EvoS.Framework.DataAccess.Daos;

namespace EvoS.Framework.DataAccess.Mock;

public class ClientErrorMockDao: ClientErrorDao
{
    public ClientErrorDao.Entry GetEntry(uint key)
    {
        return null;
    }

    public void SaveEntry(ClientErrorDao.Entry entry) { }
}