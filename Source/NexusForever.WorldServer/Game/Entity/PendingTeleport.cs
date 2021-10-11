using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;

namespace NexusForever.WorldServer.Game.Entity
{
    public class PendingTeleport
    {
        public TeleportReason Reason { get; init; }
        public MapPosition MapPosition { get; init; }
        public uint? VanityPetId { get; init; }
    }
}
