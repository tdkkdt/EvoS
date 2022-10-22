using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;

namespace EvoS.Framework.DataAccess.Daos
{
    public interface AccountDao
    {
        PersistedAccountData GetAccount(long accountId);
        void CreateAccount(PersistedAccountData data);
        void UpdateAccount(PersistedAccountData data);
    }
}