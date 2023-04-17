namespace NexusForever.Network.World.Entity
{
    public interface IEntityCommandManager
    {
        void Initialise();

        /// <summary>
        /// Return a new <see cref="IEntityCommandModel"/> of supplied <see cref="EntityCommand"/>.
        /// </summary>
        IEntityCommandModel NewEntityCommand(EntityCommand command);

        /// <summary>
        /// Returns the <see cref="EntityCommand"/> for supplied <see cref="Type"/>.
        /// </summary>
        EntityCommand? GetCommand(Type type);
    }
}