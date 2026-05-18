using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberKickMessage
    {
        public ulong GroupId { get; set; }
        public Identity Kicker { get; set; }
        public Identity Kicked { get; set; }
    }
}
