namespace CentralServer.LobbyServer.Group
{
    public class GroupRequestInfo
    {
        public readonly long RequestId;
        public readonly long RequesterAccountId;
        public readonly long RequesteeAccountId;
        public readonly long GroupId;
        // TODO expiration

        public GroupRequestInfo(long requestId, long requesterAccountId, long requesteeAccountId, long groupId)
        {
            RequestId = requestId;
            RequesterAccountId = requesterAccountId;
            RequesteeAccountId = requesteeAccountId;
            GroupId = groupId;
        }
    }
}