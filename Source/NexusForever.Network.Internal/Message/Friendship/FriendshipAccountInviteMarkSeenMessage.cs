namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountInviteMarkSeenMessage
    {
        public uint AccountId { get; set; }
        public ulong AccountFriendInviteId { get; set; }
    }
}
