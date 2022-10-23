using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.DataAccess.Mock
{
    public class AccountMockDao: AccountDao
    {
        public PersistedAccountData GetAccount(long accountId)
        {
            return null;
        }

        public void CreateAccount(PersistedAccountData data) { }

        public void UpdateAccount(PersistedAccountData data) { }
    }
}