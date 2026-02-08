using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    public class PlayerWorldZoneUpdatedMessage
    {
        public Identity Identity { get; set; }
        public ushort WorldZoneId { get; set; }
    }
}
