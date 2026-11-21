using NexusForever.Network.Internal.Message.Friendship.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipLevelUpdatedMessage
    {
        public Character Character { get; set; }
        public List<Friend> FriendsInverse { get; set; } = [];
    }
}
