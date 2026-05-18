using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Game.Abstract.Map;
using NexusForever.Game.Configuration.Model;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NLog;

namespace NexusForever.Game.Map
{
    public sealed class EntityCacheManager : Singleton<EntityCacheManager>, IEntityCacheManager
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<ushort, IEntityCache> entityCaches = new();

        public void Initialise()
        {
            log.Info("Caching map spawns...");

            List<ushort> precachedBaseMaps = SharedConfiguration.Instance.Get<MapConfig>().PrecacheBaseMaps;
            if (precachedBaseMaps == null)
                return;

            foreach (ushort worldId in precachedBaseMaps)
                LoadEntityCache(worldId);
        }

        /// <summary>
        /// Returns an existing <see cref="IEntityCache"/> for the supplied world, if it doesn't exist a new one will be created from the database.
        /// </summary>
        public IEntityCache GetEntityCache(ushort worldId)
        {
            if (entityCaches.TryGetValue(worldId, out IEntityCache entityCache))
                return entityCache;

            return LoadEntityCache(worldId);
        }

        private IEntityCache LoadEntityCache(ushort worldId)
        {
            var entityCache = new EntityCache(DatabaseManager.Instance.GetDatabase<WorldDatabase>().GetEntities(worldId));
            entityCaches.Add(worldId, entityCache);

            log.Trace($"Initialised {entityCache.EntityCount} spawns on {entityCache.GridCount} grids for world {worldId}.");
            return entityCache;
        }
    }
}
