using System.Collections;
using System.Collections.Concurrent;
using NexusForever.Game.Abstract.Map.Lock;

namespace NexusForever.Game.Map.Lock
{
    public class MapLockCollection : IMapLockCollection
    {
        private ConcurrentDictionary<uint, IMapLock> locks = [];

        /// <summary>
        /// Return <see cref="IMapLock"/> for supplied world id.
        /// </summary>
        public IMapLock GetMapLock<T>(uint worldId) where T : IMapLock
        {
            return locks.TryGetValue(worldId, out IMapLock mapLock) ? mapLock : null;
        }

        /// <summary>
        /// Add <see cref="IMapLock"/> to collection.
        /// </summary>
        public void AddMapLock(IMapLock mapLock)
        {
            if (locks.ContainsKey(mapLock.WorldId))
                throw new InvalidOperationException();

            locks[mapLock.WorldId] = mapLock;
        }

        /// <summary>
        /// Remove <see cref="IMapLock"/> from collection with the supplied world id key.
        /// </summary>
        public void RemoveMapLock(uint worldId)
        {
            locks.TryRemove(worldId, out _);
        }

        public IEnumerator<IMapLock> GetEnumerator()
        {
            return locks.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
