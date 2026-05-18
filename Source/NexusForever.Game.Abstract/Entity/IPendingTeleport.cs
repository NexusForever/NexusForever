using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPendingTeleport
    {
        TeleportReason Reason { get; init; }
        IMapPosition MapPosition { get; init; }
        uint? VanityPetId { get; init; }
    }
}