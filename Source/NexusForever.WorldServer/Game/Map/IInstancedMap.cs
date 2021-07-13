using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Map
{
    public interface IInstancedMap
    {
        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be added to <see cref="IMap"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="Player.TeleportTo(MapPosition, TeleportReason)"/> instead.
        /// </remarks>
        void EnqueueAdd(Player player, MapPosition position);
    }
}
