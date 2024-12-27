using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IEntityFactory
    {
        /// <summary>
        /// Create a new entity of type <typeparamref name="T"/>.
        /// </summary>
        T CreateEntity<T>() where T : IGridEntity;

        /// <summary>
        /// Create a new <see cref="IWorldEntity"/> of supplied <see cref="EntityType"/>.
        /// </summary>
        /// <remarks>
        /// This should only be used for creating entities from the database, otherwise use <see cref="CreateEntity{T}"/>.
        /// </remarks>
        IWorldEntity CreateWorldEntity(EntityType type);
    }
}
