using NexusForever.Network.Internal.Message.Friendship.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountPresenceUpdatedMessage
    {
        public Account Account { get; set; }
        public List<FriendAccount> FriendsInverse { get; set; } = [];
    }
}
