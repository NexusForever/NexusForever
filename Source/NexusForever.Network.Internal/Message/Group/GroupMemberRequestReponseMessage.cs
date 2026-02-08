using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupMemberRequestReponseMessage
    {
        public ulong GroupId { get; set; }
        public Identity Identity { get; set; }
        public string InviteeName { get; set; }
        public bool Response { get; set; }
    }
}
