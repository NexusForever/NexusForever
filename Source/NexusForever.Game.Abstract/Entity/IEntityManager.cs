using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IEntityManager
    {
        void Initialise();

        /// <summary>
        /// Return a new <see cref="IWorldEntity"/> of supplied <see cref="EntityType"/>.
        /// </summary>
        IWorldEntity NewEntity(EntityType type);

        StatAttribute GetStatAttribute(Stat stat);
    }
}