using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Static;

namespace NexusForever.WorldServer.Game.Map
{
    public interface IMap : IUpdate
    {
        /// <summary>
        /// Initialise <see cref="IMap"/> with <see cref="WorldEntry"/>.
        /// </summary>
        void Initialise(WorldEntry entry);

        /// <summary>
        /// Enqueue <see cref="GridEntity"/> to be added to <see cref="IMap"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="Player.TeleportTo(MapPosition, TeleportReason)"/> instead.
        /// </remarks>
        void EnqueueAdd(GridEntity entity, MapPosition position);

        /// <summary>
        /// Returns if <see cref="GridEntity"/> can be added to <see cref="IMap"/>.
        /// </summary>
        bool CanEnter(GridEntity entity, MapPosition position);

        /// <summary>
        /// Returns if <see cref="Player"/> can be added to <see cref="IMap"/>.
        /// </summary>
        GenericError? CanEnter(Player entity, MapPosition position);
    }
}
