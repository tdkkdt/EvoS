using EvoS.Framework.DataAccess.Daos;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using log4net;

namespace EvoS.Framework.DataAccess.Mongo
{
    public class AccountMongoDao : MongoDao<long, PersistedAccountData>, AccountDao
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AccountMongoDao));
        
        public AccountMongoDao() : base("accounts")
        {
        }

        public PersistedAccountData GetAccount(long accountId)
        {
            return findById(accountId);
        }

        public void CreateAccount(PersistedAccountData data)
        {
            log.Info($"New player {data.AccountId}: {data}");
            UpdateAccount(data);
        }

        public void UpdateAccount(PersistedAccountData data)
        {
            insert(data.AccountId, data);
        }

        private static readonly FieldDefinition<AccountComponent> FAccountComponent = new(x => x.AccountComponent);
        public void UpdateAccountComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FAccountComponent);
        }

        private static readonly FieldDefinition<AdminComponent> FAdminComponent = new(x => x.AdminComponent);
        public void UpdateAdminComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FAdminComponent);
        }

        private static readonly FieldDefinition<BankComponent> FBankComponent = new(x => x.BankComponent);
        public void UpdateBankComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FBankComponent);
        }

        private static readonly FieldDefinition<SocialComponent> FSocialComponent = new(x => x.SocialComponent);
        public void UpdateSocialComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FSocialComponent);
        }

        private static readonly FieldDefinition<CharacterType> FLastCharacter = new(x => x.AccountComponent.LastCharacter);
        public void UpdateLastCharacter(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FLastCharacter);
        }
        
        public void UpdateCharacterComponent(PersistedAccountData data, CharacterType characterType)
        {
            c.UpdateOne(
                Key(data.AccountId), 
                u.Set(
                    account => account.CharacterData[characterType].CharacterComponent, 
                    data.CharacterData[characterType].CharacterComponent));
        }
    }
} 