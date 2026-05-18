using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Group.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberLeftMessage
    {
        public Shared.Group Group { get; set; }
        public GroupMember RemovedMember { get; set; }
        public RemoveReason Reason { get; set; }
    }
}
