using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Map.Lock;

namespace NexusForever.Game.Abstract.Map.Lock
{
    public interface IMapLockManager
    {
        void Initialise();

        /// <summary>
        /// Create a new solo <see cref="IMapLock"/> for supplied character id and world id.
        /// </summary>
        IMapLock CreateSoloLock(ulong characterId, uint worldId);

        /// <summary>
        /// Create a new match <see cref="IMapLock"/> for supplied <see cref="IMatch"/>.
        /// </summary>
        IMapLock CreateMatchLock(IMatch match);

        /// <summary>
        /// Return <see cref="IMapLock"/> for supplied character id and world id.
        /// </summary>
        IMapLock GetSoloLock(ulong characterId, uint worldId);

        /// <summary>
        /// Return <see cref="IMapLock"/> for supplied match guid and world id.
        /// </summary>
        IMapLock GetMatchLock(Guid guid, uint worldId);

        /// <summary>
        /// Return <see cref="IResidenceMapLock"/> for supplied <see cref="IResidence"/>.
        /// </summary>
        IResidenceMapLock GetResidenceLock(IResidence residence);
    }
}
