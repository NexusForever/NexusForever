using NexusForever.Game.Map;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class PendingTeleport
    {
        public TeleportReason Reason { get; init; }
        public MapPosition MapPosition { get; init; }
        public uint? VanityPetId { get; init; }
    }
}
