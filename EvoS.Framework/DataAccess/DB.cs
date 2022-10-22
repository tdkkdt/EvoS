using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.DataAccess.Mongo;

namespace EvoS.Framework.DataAccess
{
    public class DB
    {
        private static DB Instance;
        public readonly AccountDao AccountDao;

        private DB()
        {
            AccountDao = new AccountMongoDao();
        }

        public static DB Get()
        {
            return Instance ??= new DB();
        }
    }
}