using NexusForever.Network.Internal.Message.Friendship.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountLocationsUpdatedMessage
    {
        public Account Account { get; set; }
        public List<FriendAccount> Friends { get; set; } = [];
    }
}
