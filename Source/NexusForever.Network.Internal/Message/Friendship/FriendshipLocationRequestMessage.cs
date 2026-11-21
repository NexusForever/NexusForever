using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipLocationRequestMessage
    {
        public Identity Identity { get; set; }
        public List<Identity> Friends { get; set; }
        public List<ulong> AccountFriends { get; set; }
    }
}
