using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipRemoveIdentityMessage
    {
        public Identity Inviter { get; set; }
        public Identity Invitee { get; set; }
        public FriendshipType Type { get; set; }
    }
}
