using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Map
{
    public interface IMap : IUpdate
    {
        /// <summary>
        /// Initialise <see cref="IMap"/> with <see cref="WorldEntry"/>.
        /// </summary>
        void Initialise(WorldEntry entry);

        /// <summary>
        /// Enqueue <see cref="IGridEntity"/> to be added to <see cref="IMap"/>.
        /// </summary>
        /// <remarks>
        /// Characters should not be added directly through this method.
        /// Use <see cref="IPlayer.TeleportTo(IMapPosition, TeleportReason)"/> instead.
        /// </remarks>
        void EnqueueAdd(IGridEntity entity, IMapPosition position);

        /// <summary>
        /// Returns if <see cref="GridEntity"/> can be added to <see cref="IMap"/>.
        /// </summary>
        bool CanEnter(IGridEntity entity, IMapPosition position);

        /// <summary>
        /// Returns if <see cref="Player"/> can be added to <see cref="IMap"/>.
        /// </summary>
        GenericError? CanEnter(IPlayer entity, IMapPosition position);
    }
}
