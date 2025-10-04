using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupActionResultMessage
    {
        public ulong GroupId { get; set; }
        public Identity Recipient { get; set; }
        public Identity Target { get; set; }
        public GroupActionResult Result { get; set; }
    }
}
