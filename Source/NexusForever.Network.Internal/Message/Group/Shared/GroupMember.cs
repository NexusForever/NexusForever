using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group.Shared
{
    public class GroupMember
    {
        public Identity Identity { get; set; }
        public GroupMemberInfoFlags Flags { get; set; }
        public GroupCharacter Character { get; set; }
        public uint GroupIndex { get; set; }
    }
}
