using NexusForever.Network.Internal.Message.Friendship.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipLastOnlineUpdatedMessage
    {
        public Character Character { get; set; }
        public List<Friend> FriendsInverse { get; set; } = [];
    }
}
