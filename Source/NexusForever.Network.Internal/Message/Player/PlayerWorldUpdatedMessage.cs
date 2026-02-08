using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    public class PlayerWorldUpdatedMessage
    {
        public Identity Identity { get; set; }
        public uint WorldId { get; set; }
    }
}
