using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupFlagsUpdateMessage
    {
        public ulong GroupId { get; set; }
        public Identity Identity { get; set; }
        public GroupFlags Flags { get; set; }
    }
}
