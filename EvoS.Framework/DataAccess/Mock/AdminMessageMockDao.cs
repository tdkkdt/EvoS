using System.Collections.Generic;
using EvoS.Framework.DataAccess.Daos;

namespace EvoS.Framework.DataAccess.Mock
{
    public class AdminMessageMockDao: AdminMessageDao
    {
        public AdminMessageDao.AdminMessage FindPending(long accountId)
        {
            return null;
        }

        List<AdminMessageDao.AdminMessage> AdminMessageDao.Find(long accountId)
        {
            return new List<AdminMessageDao.AdminMessage>();
        }

        public void Save(AdminMessageDao.AdminMessage msg)
        {
        }
    }
}