using NexusForever.Network.Internal.Message.Friendship.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountInviteListMessage
    {
        public Account Account { get; set; }
        public List<FriendAccountInvite> Invites { get; set; } = [];
    }
}
