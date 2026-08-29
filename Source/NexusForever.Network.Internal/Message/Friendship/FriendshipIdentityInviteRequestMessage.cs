using NexusForever.Game.Static.Friendship;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipIdentityInviteRequestMessage
    {
        public Identity Inviter { get; set; }
        public Identity Invitee { get; set; }
        public FriendshipType Type { get; set; }
        public string Note { get; set; }
    }
}
