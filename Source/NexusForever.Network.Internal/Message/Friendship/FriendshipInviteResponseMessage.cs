using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipInviteResponseMessage
    {
        public Identity Invitee { get; set; }
        public ulong InviteId { get; set; }
        public FriendshipResponse Response { get; set; }
    }
}
