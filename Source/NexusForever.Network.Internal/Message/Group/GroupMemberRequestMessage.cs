using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberRequestMessage
    {
        public Identity Requester { get; set; }
        public IdentityName Requestee { get; set; }
    }
}
