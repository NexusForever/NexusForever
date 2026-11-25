using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IPendingTeleport
    {
        TeleportReason Reason { get; init; }
        IMapPosition MapPosition { get; init; }
        uint? VanityPetId { get; init; }
        bool Resurrect { get; init; }

        /// <summary>
        /// Position in the open world map being left for another world, applied as the return position once the teleport succeeds.
        /// </summary>
        IMapPosition ReturnPosition { get; init; }
    }
}