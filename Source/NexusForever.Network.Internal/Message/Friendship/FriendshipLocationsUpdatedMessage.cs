using NexusForever.Network.Internal.Message.Friendship.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipLocationsUpdatedMessage
    {
        public Character Character { get; set; }
        public List<Friend> Friends { get; set; } = [];
    }
}
