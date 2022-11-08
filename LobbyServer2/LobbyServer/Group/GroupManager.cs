using System.Collections.Generic;
using CentralServer.LobbyServer.Session;
using EvoS.Framework.DataAccess;
using EvoS.Framework.Network.Static;

namespace CentralServer.LobbyServer.Group
{
    class GroupManager
    {
        public static LobbyPlayerGroupInfo GetGroupInfo(long accountId)
        {
            // TODO
            LobbyServerProtocol client = SessionManager.GetClientConnection(accountId);

            LobbyPlayerGroupInfo groupInfo = new LobbyPlayerGroupInfo()
            {
                SelectedQueueType = client.SelectedGameType,
                MemberDisplayName = client.UserName,
                //InAGroup = false,
                //IsLeader = true,
                Members = new List<UpdateGroupMemberData>(),
            };
            
            PersistedAccountData account = DB.Get().AccountDao.GetAccount(accountId);
            groupInfo.SetCharacterInfo(LobbyCharacterInfo.Of(account.CharacterData[account.AccountComponent.LastCharacter]));
            
            return groupInfo;
        }
    }
}
