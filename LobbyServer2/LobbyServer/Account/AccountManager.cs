using CentralServer.LobbyServer.Character;
using CentralServer.LobbyServer.Inventory;
using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Network.NetworkMessages;
using EvoS.Framework.Network.Static;
using System;
using System.Collections.Generic;
using System.Text;

namespace CentralServer.LobbyServer.Account
{
    public class AccountManager
    {
        public static PersistedAccountData CreateAccount(AssignGameClientRequest request)
        {
            long accountId = request.AuthInfo.AccountId;
            PersistedAccountData accountData = new PersistedAccountData
            {
                AccountComponent = GetAccountComponent(accountId),
                AccountId = accountId,
                BankComponent = Bank.GetBankComponent(accountId),
                CharacterData = CharacterManager.GetPersistedCharacterData(accountId),
                Handle = request.AuthInfo.Handle,
                InventoryComponent = InventoryManager.GetInventoryComponent(accountId),
                QuestComponent = new QuestComponent()
                {
                    ActiveSeason = 9,
                    SeasonExperience = new Dictionary<int, ExperienceComponent>()
                    {
                        {9, new ExperienceComponent()}
                    }
                },
                SchemaVersion = new SchemaVersion<AccountSchemaChange>(0x1FFFF),
                UserName = request.AuthInfo.UserName
            };

            return accountData;
        }
        public static AccountComponent GetAccountComponent(long accountId)
        {
            AccountComponent accountComponent = new AccountComponent()
            {
                AppliedEntitlements = new Dictionary<string, int>(),
                DailyQuestsAvailable = Config.ConfigManager.DailyQuestsAvailable,
                DisplayDevTag = false,
                FactionCompetitionData = new Dictionary<int, PlayerFactionCompetitionData>(),
                FreeRotationCharacters = new CharacterType[] { },
                LastCharacter = Config.ConfigManager.DefaultCharacterType,
                SelectedBackgroundBannerID = -1,
                SelectedForegroundBannerID = -1,
                SelectedRibbonID = -1,
                SelectedTitleID = -1,
                UnlockedBannerIDs = InventoryManager.GetUnlockedBannerIDs(accountId),
                UIStates = new Dictionary<AccountComponent.UIStateIdentifier, int>
                {
                    { AccountComponent.UIStateIdentifier.HasViewedFluxHighlight, 1 },
                    { AccountComponent.UIStateIdentifier.HasViewedGGHighlight, 1 }
                },
                UnlockedEmojiIDs = InventoryManager.GetUnlockedEmojiIDs(accountId),
                UnlockedLoadingScreenBackgroundIdsToActivatedState = InventoryManager.GetActivatedLoadingScreenBackgroundIds(accountId),
                UnlockedOverconIDs = InventoryManager.GetUnlockedOverconIDs(accountId),
                UnlockedTitleIDs = InventoryManager.GetUnlockedTitleIDs(accountId),
                UnlockedRibbonIDs = InventoryManager.GetUnlockedRibbonIDs(accountId)
            };

            return accountComponent;
        }
    }
}
