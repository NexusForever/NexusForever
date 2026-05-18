namespace NexusForever.Game.Abstract.Map
{
    public interface IEntityCacheManager
    {
        void Initialise();

        /// <summary>
        /// Returns an existing <see cref="IEntityCache"/> for the supplied world, if it doesn't exist a new one will be created from the database.
        /// </summary>
        IEntityCache GetEntityCache(ushort worldId);
    }
}