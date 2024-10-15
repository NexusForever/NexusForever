using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Map.Instance;
using NexusForever.Game.Abstract.Map.Lock;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Static.Map.Lock;
using NexusForever.Shared;

namespace NexusForever.Game.Map.Instance
{
    public class ContentInstancedMap<T> : InstancedMap<T> where T : class, IContentMapInstance
    {
        #region Dependency Injection;

        private readonly IMapLockManager mapLockManager;
        private readonly IMatchManager matchManager;
        private readonly IFactory<T> instanceFactory;
        private readonly IMatchingDataManager matchingDataManager;

        public ContentInstancedMap(
            ILogger<ContentInstancedMap<T>> log,
            IMapLockManager mapLockManager,
            IMatchManager matchManager,
            IFactory<T> instanceFactory,
            IMatchingDataManager matchingDataManager)
            : base(log)
        {
            this.mapLockManager      = mapLockManager;
            this.matchManager        = matchManager;
            this.instanceFactory     = instanceFactory;
            this.matchingDataManager = matchingDataManager;
        }

        #endregion

        protected override IMapLock GetMapLock(IPlayer player)
        {
            IMatch match = matchManager.GetMatchCharacter(player.CharacterId).Match;
            if (match != null)
            {
                IMapLock matchMapLock = mapLockManager.GetMatchLock(match.Guid, Entry.Id);
                return matchMapLock ?? mapLockManager.CreateMatchLock(match);
            }

            // TODO: check group lock

            IMapLock soloMapLock = mapLockManager.GetSoloLock(player.CharacterId, Entry.Id);
            return soloMapLock ?? mapLockManager.CreateSoloLock(player.CharacterId, Entry.Id);
        }

        protected override T CreateInstance(IPlayer player, IMapLock mapLock)
        {
            T instance = instanceFactory.Resolve();
            instance.Initialise(Entry, mapLock);

            // it is possible for a content map lock to not be a match lock
            // this could occur if a player or party enters a map via the instance portal
            if (mapLock.Type == MapLockType.Match)
            {
                IMatch match = matchManager.GetMatchCharacter(player.CharacterId).Match;
                if (match != null)
                    instance.SetMatch(match);
            }

            return instance;
        }
    }
}
