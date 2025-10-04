using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberRequestResultMessage
    {
        public Identity Recipient { get; set; }
        public IdentityName Target { get; set; }
        public GroupInviteResult Result { get; set; }
        public GroupRequestType Type { get; set; }
    }
}
