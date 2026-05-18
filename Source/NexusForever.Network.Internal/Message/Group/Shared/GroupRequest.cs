using NexusForever.Game.Static.Group;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group.Shared
{
    public class GroupRequest
    {
        public Identity Requester { get; set; }
        public Identity Requestee { get; set; }
        public GroupRequestType Type { get; set; }
        public DateTime Expiration { get; set; }
    }
}
