namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountRemoveMessage
    {
        public uint AccountId { get; set; }
        public ulong AccountFriendId { get; set; }
    }
}
