using EvoS.Framework.Network.Static;

namespace EvoS.Framework.DataAccess.Daos
{
    public interface AccountDao
    {
        PersistedAccountData GetAccount(long accountId);
        void CreateAccount(PersistedAccountData data);
        void UpdateAccount(PersistedAccountData data);

        void UpdateAccountComponent(PersistedAccountData data)
        {
            UpdateAccount(data);
        }

        void UpdateAdminComponent(PersistedAccountData data)
        {
            UpdateAccount(data);
        }

        void UpdateBankComponent(PersistedAccountData data)
        {
            UpdateAccount(data);
        }

        void UpdateSocialComponent(PersistedAccountData data)
        {
            UpdateAccount(data);
        }

        void UpdateLastCharacter(PersistedAccountData data)
        {
            UpdateAccount(data);
        }

        void UpdateCharacterComponent(PersistedAccountData data, CharacterType characterType)
        {
            UpdateAccount(data);
        }
    }
}