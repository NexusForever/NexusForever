using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountInviteResponseMessage
    {
        public Identity Invitee { get; set; }
        public uint AccountId { get; set; }
        public ulong InviteId { get; set; }
        public bool Response { get; set; }
    }
}
