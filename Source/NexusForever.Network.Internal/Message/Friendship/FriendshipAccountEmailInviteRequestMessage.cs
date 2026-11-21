using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountEmailInviteRequestMessage
    {
        public Identity Inviter { get; set; }
        public uint InviterAccountId { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
    }
}
