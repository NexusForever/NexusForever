using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Friendship
{
    public class FriendshipAccountFriendInviteRequestMessage
    {
        public Identity Inviter { get; set; }
        public uint InviterAccountId { get; set; }
        public Identity Invitee { get; set; }
        public string Note { get; set; }
    }
}
