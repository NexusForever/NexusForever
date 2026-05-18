using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipInviteMarkSeenMessage
    {
        public Identity Identity { get; set; }
        public List<ulong> FriendInviteIds { get; set; } = [];
    }
}
