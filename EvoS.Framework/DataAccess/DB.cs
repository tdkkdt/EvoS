using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.DataAccess.Mock;
using EvoS.Framework.DataAccess.Mongo;
using log4net;

namespace EvoS.Framework.DataAccess
{
    public class DB
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DB));
        
        private static DB Instance;
        public readonly AccountDao AccountDao;

        private DB()
        {
            switch (EvosConfiguration.GetDBConfig().Type)
            {
                case EvosConfiguration.DBType.Mongo:
                    log.Info("Using MongoDB");
                    AccountDao = new AccountDaoCached(new AccountMongoDao());
                    break;
                case EvosConfiguration.DBType.None:
                    log.Info("Not using any database, no data will be persisted");
                    AccountDao = new AccountDaoCached(new AccountMockDao());
                    break;
            }
        }

        public static DB Get()
        {
            return Instance ??= new DB();
        }
    }
}