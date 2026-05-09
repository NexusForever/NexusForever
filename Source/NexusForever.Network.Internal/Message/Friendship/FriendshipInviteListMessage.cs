using NexusForever.Network.Internal.Message.Friendship.Shared;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipInviteListMessage
    {
        public Identity Identity { get; set; }
        public List<FriendInvite> Invites { get; set; } = [];
    }
}
