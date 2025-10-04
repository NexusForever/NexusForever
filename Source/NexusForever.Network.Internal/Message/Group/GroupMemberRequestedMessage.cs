using NexusForever.Network.Internal.Message.Group.Shared;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberRequestedMessage
    {
        public Shared.Group Group { get; set; }
        public Identity RequesterIdentity { get; set; }
        public GroupCharacter Requester { get; set; }
    }
}
