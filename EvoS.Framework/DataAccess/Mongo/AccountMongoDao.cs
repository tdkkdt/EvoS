using System;
using System.Collections.Generic;
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
            UpdateUpdateTime(data);
        }

        private static readonly FieldDefinition<DateTime> FUpdateDate = new(x => x.UpdateDate);
        private void UpdateUpdateTime(PersistedAccountData data)
        {
            data.UpdateDate = DateTime.UtcNow;
            UpdateField(data.AccountId, data, FUpdateDate);
        }
        
        private static readonly FieldDefinition<AccountComponent> FAccountComponent = new(x => x.AccountComponent);
        public void UpdateAccountComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FAccountComponent);
            UpdateUpdateTime(data);
        }

        private static readonly FieldDefinition<AdminComponent> FAdminComponent = new(x => x.AdminComponent);
        public void UpdateAdminComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FAdminComponent);
            UpdateUpdateTime(data);
        }

        private static readonly FieldDefinition<BankComponent> FBankComponent = new(x => x.BankComponent);
        public void UpdateBankComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FBankComponent);
            UpdateUpdateTime(data);
        }

        private static readonly FieldDefinition<SocialComponent> FSocialComponent = new(x => x.SocialComponent);
        public void UpdateSocialComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FSocialComponent);
            UpdateUpdateTime(data);
        }

        private static readonly FieldDefinition<CharacterType> FLastCharacter = new(x => x.AccountComponent.LastCharacter);
        public void UpdateLastCharacter(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FLastCharacter);
            UpdateUpdateTime(data);
        }
        
        private static readonly FieldDefinition<Dictionary<CharacterType, PersistedCharacterData>> FCharacterData = new(x => x.CharacterData);
        public void UpdateCharacterComponent(PersistedAccountData data, CharacterType characterType)
        {
            UpdateField(data.AccountId, data, FCharacterData);
            UpdateUpdateTime(data);
        }

        private static readonly FieldDefinition<ExperienceComponent> FExperienceComponent = new(x => x.ExperienceComponent);
        public void UpdateExperienceComponent(PersistedAccountData data)
        {
            UpdateField(data.AccountId, data, FExperienceComponent);
            UpdateUpdateTime(data);
        }
    }
} 