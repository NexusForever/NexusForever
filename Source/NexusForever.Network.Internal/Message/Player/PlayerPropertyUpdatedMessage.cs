using NexusForever.Game.Static.Entity;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    public class PlayerPropertyUpdatedMessage
    {
        public Identity Identity { get; set; }
        public Property Property { get; set; }
        public float Value { get; set; }
    }
}
