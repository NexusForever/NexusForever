using System.Collections.Concurrent;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Housing;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Static.Map.Lock;
using NexusForever.Shared;

namespace NexusForever.Game.Map.Lock
{
    // legacy singleton still required for guild operations which don't use dependency injection yet...
    public class MapLockManager : Singleton<IMapLockManager>, IMapLockManager
    {
        private readonly ConcurrentDictionary<Identity, IMapLockCollection> soloLocks = [];
        private readonly ConcurrentDictionary<Guid, IMapLockCollection> matchLocks = [];
        private readonly ConcurrentDictionary<ulong, IResidenceMapLock> residenceLocks = [];

        #region Dependency Injection

        private readonly IFactoryInterface<IMapLock> mapLockFactory;
        private readonly IFactory<IMapLockCollection> mapLockCollectionFactory;

        public MapLockManager(
            IFactoryInterface<IMapLock> mapLockFactory,
            IFactory<IMapLockCollection> mapLockCollectionFactory)
        {
            this.mapLockFactory           = mapLockFactory;
            this.mapLockCollectionFactory = mapLockCollectionFactory;
        }

        #endregion

        public void Initialise()
        {
            // TODO: load locks from database
        }

        /// <summary>
        /// Create a new solo <see cref="IMapLock"/> for supplied character id and world id.
        /// </summary>
        public IMapLock CreateSoloLock(Identity identity, uint worldId)
        {
            IMapLock mapLock = CreateLock<IMapLock>(MapLockType.Solo, worldId);
            mapLock.AddCharacer(identity);

            if (!soloLocks.TryGetValue(identity, out IMapLockCollection mapLockCollection))
            {
                mapLockCollection = mapLockCollectionFactory.Resolve();
                soloLocks[identity] = mapLockCollection;
            }

            mapLockCollection.AddMapLock(mapLock);
            return mapLock;
        }

        /// <summary>
        /// Create a new match <see cref="IMapLock"/> for supplied <see cref="IMatch"/>.
        /// </summary>
        public IMapLock CreateMatchLock(IMatch match)
        {
            IMapLock mapLock = CreateLock<IMapLock>(MapLockType.Match, match.MatchingMap.GameMapEntry.WorldId);

            foreach (IMatchTeam matchTeam in match.GetTeams())
                foreach (IMatchTeamMember matchTeamMember in matchTeam.GetMembers())
                    mapLock.AddCharacer(matchTeamMember.Identity);

            if (!matchLocks.TryGetValue(match.Guid, out IMapLockCollection mapLockCollection))
            {
                mapLockCollection = mapLockCollectionFactory.Resolve();
                matchLocks[match.Guid] = mapLockCollection;
            }

            mapLockCollection.AddMapLock(mapLock);
            return mapLock;
        }

        private T CreateLock<T>(MapLockType lockType, uint worldId) where T : IMapLock
        {
            IMapLock mapLock = mapLockFactory.Resolve<T>();
            mapLock.Initialise(lockType, worldId);
            return (T)mapLock;
        }

        /// <summary>
        /// Return <see cref="IMapLock"/> for supplied character id and world id.
        /// </summary>
        public IMapLock GetSoloLock(Identity identity, uint worldId)
        {
            return soloLocks.TryGetValue(identity, out IMapLockCollection mapLockCollection)
                ? mapLockCollection.GetMapLock<IMapLock>(worldId)
                : null;
        }

        /// <summary>
        /// Return <see cref="IMapLock"/> for supplied match guid and world id.
        /// </summary>
        public IMapLock GetMatchLock(Guid guid, uint worldId)
        {
            return matchLocks.TryGetValue(guid, out IMapLockCollection mapLockCollection)
                ? mapLockCollection.GetMapLock<IMapLock>(worldId)
                : null;
        }

        /// <summary>
        /// Return <see cref="IResidenceMapLock"/> for supplied <see cref="IResidence"/>.
        /// </summary>
        public IResidenceMapLock GetResidenceLock(IResidence residence)
        {
            if (residenceLocks.TryGetValue(residence.Id, out IResidenceMapLock mapLock))
                return mapLock;

            mapLock = CreateLock<IResidenceMapLock>(MapLockType.Residence, 0u);
            mapLock.Initialise(residence.Id);
            residenceLocks[residence.Id] = mapLock;
            return mapLock;
        }
    }
}
