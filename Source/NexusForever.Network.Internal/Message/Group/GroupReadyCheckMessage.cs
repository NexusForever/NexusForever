using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Group
{
    public class GroupReadyCheckMessage
    {
        public ulong GroupId { get; set; }
        public Identity Initator { get; set; }
        public string Message { get; set; }
    }
}
