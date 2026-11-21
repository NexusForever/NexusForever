using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountStatusUpdateMessage
    {
        public Identity Identity { get; set; }
        public uint AccountId { get; set; }
        public string Status { get; set; }
    }
}
