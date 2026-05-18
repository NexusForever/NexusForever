using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupPlayerInviteMessage
    {
        public Identity Inviter { get; set; }
        public IdentityName Invitee { get; set; }
    }
}
