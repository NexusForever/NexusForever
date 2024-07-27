namespace NexusForever.Game.Abstract.Map.Lock
{
    public interface IMapLockCollection : IEnumerable<IMapLock>
    {
        /// <summary>
        /// Return <see cref="IMapLock"/> for supplied world id.
        /// </summary>
        IMapLock GetMapLock<T>(uint worldId) where T : IMapLock;

        /// <summary>
        /// Add <see cref="IMapLock"/> to collection.
        /// </summary>
        void AddMapLock(IMapLock mapLock);

        /// <summary>
        /// Remove <see cref="IMapLock"/> from collection with the supplied world id key.
        /// </summary>
        void RemoveMapLock(uint worldId);
    }
}
