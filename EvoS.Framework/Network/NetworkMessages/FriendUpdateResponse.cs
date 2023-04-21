using EvoS.Framework.Constants.Enums;
using EvoS.Framework.Network.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoS.Framework.Network.NetworkMessages
{
    [Serializable]
    [EvosMessage(362)]
    public class FriendUpdateResponse : WebSocketResponseMessage
    {
        public long FriendAccountId;
        public string FriendHandle;
        public FriendOperation FriendOperation;
        public LocalizationPayload LocalizedFailure;

        public static FriendUpdateResponse of(FriendUpdateRequest request, bool success)
        {
            return new FriendUpdateResponse
            {
                FriendAccountId = request.FriendAccountId,
                FriendHandle = request.FriendHandle,
                FriendOperation = request.FriendOperation,
                Success = success,
                ResponseId = request.RequestId
            };
        }
    }
}
