using NexusForever.Game.Static.Entity;
using NexusForever.Network.Internal.Message.Shared;

namespace NexusForever.Network.Internal.Message.Player
{
    public class PlayerStatUpdatedMessage
    {
        public Identity Identity { get; set; }
        public Stat Stat { get; set; }
        public float Value { get; set; }
    }
}
