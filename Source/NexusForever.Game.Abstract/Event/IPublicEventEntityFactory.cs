using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventEntityFactory
    {
        /// <summary>
        /// Initalise <see cref="IPublicEventEntityFactory"/> with the specified <see cref="IPublicEvent"/>.
        /// </summary>
        void Initialise(IPublicEvent publicEvent);

        /// <summary>
        /// Spawn entities for the specified <see cref="IPublicEvent"/> phase.
        /// </summary>
        void SpawnEntities(uint phase);

        /// <summary>
        /// Remove all entities spawned for the <see cref="IPublicEvent"/>.
        /// </summary>
        void RemoveEntities();

        /// <summary>
        /// Create a new <see cref="IGridEntity"/> that belongs to the <see cref="IPublicEvent"/>.
        /// </summary>
        T CreateEntity<T>() where T : IGridEntity;
    }
}
