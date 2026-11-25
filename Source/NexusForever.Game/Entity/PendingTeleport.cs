using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class PendingTeleport : IPendingTeleport
    {
        public TeleportReason Reason { get; init; }
        public IMapPosition MapPosition { get; init; }
        public uint? VanityPetId { get; init; }
        public bool Resurrect { get; init; }
        public IMapPosition ReturnPosition { get; init; }
    }
}
