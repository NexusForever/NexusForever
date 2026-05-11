using NexusForever.Network.Internal.Message.Friendship.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountUpdatedMessage
    {
        public Account Account { get; set; }
        public List<FriendAccount> FriendsInverse { get; set; } = [];
    }
}
