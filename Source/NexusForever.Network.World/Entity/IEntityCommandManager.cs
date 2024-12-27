using NexusForever.Game.Static.Entity.Movement.Command;

namespace NexusForever.Network.World.Entity
{
    public interface IEntityCommandManager
    {
        void Initialise();

        /// <summary>
        /// Return a new <see cref="IEntityCommandModel"/> of supplied <see cref="EntityCommand"/>.
        /// </summary>
        IEntityCommandModel NewEntityCommand(EntityCommand id);
    }
}