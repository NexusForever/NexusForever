using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupPlayerInvitedMessage
    {
        public Shared.Group Group { get; set; }
        public Shared.GroupMember Leader { get; set; }
        public Shared.GroupMember Inviter { get; set; }
        public Identity InviteeIdentity { get; set; }
        public Shared.GroupCharacter Invitee { get; set; }
    }
}
