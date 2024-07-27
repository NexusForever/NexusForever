using NexusForever.Game.Static.Map.Lock;

namespace NexusForever.Game.Abstract.Map.Lock
{
    public interface IMapLock
    {
        Guid InstanceId { get; }
        MapLockType Type { get; }
        uint WorldId { get; }

        /// <summary>
        /// Initialise new map lock with supplied <see cref="MapLockType"/> and world id.
        /// </summary>
        void Initialise(MapLockType mapLockType, uint worldId);

        /// <summary>
        /// Add a character to the map lock.
        /// </summary>
        void AddCharacer(ulong characterId);

        /// <summary>
        /// Remove a character from the map lock.
        /// </summary>
        void RemoveCharacter(ulong characterId);
    }
}
