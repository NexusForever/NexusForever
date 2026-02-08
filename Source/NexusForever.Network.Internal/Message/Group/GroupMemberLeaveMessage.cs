using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberLeaveMessage
    {
        public Identity Identity { get; set; }
        public ulong GroupId { get; set; }
    }
}
