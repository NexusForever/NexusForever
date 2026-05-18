using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberFlagUpdateMessage
    {
        public ulong GroupId { get; set; }
        public Identity Source { get; set; }
        public Identity Target { get; set; }
        public GroupMemberInfoFlags Flags { get; set; }
    }
}
