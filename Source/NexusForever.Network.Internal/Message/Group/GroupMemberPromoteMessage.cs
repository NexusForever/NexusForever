using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberPromoteMessage
    {
        public ulong GroupId { get; set; }
        public Identity Promoter { get; set; }
        public Identity Promotee { get; set; }
    }
}
