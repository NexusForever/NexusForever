using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.IO.Map;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Entity
{
    public sealed class EntityManager : Singleton<EntityManager>, IEntityManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ImmutableDictionary<Stat, StatAttribute> statAttributes;

        public void Initialise()
        {
            InitialiseEntityStats();

            CalculateEntityAreaData();
        }

        private void InitialiseEntityStats()
        {
            var builder = ImmutableDictionary.CreateBuilder<Stat, StatAttribute>();

            foreach (FieldInfo field in typeof(Stat).GetFields())
            {
                StatAttribute attribute = field.GetCustomAttribute<StatAttribute>();
                if (attribute == null)
                    continue;

                Stat stat = (Stat)field.GetValue(null);
                builder.Add(stat, attribute);
            }

            statAttributes = builder.ToImmutable();
        }

        [Conditional("DEBUG")]
        private void CalculateEntityAreaData()
        {
            log.Info("Calculating area information for entities...");

            var mapFiles = new Dictionary<ushort, MapFile>();
            var entities = new HashSet<EntityModel>();

            foreach (EntityModel model in DatabaseManager.Instance.GetDatabase<WorldDatabase>().GetEntitiesWithoutArea())
            {
                entities.Add(model);

                if (!mapFiles.TryGetValue(model.World, out MapFile mapFile))
                {
                    WorldEntry entry = GameTableManager.Instance.World.GetEntry(model.World);
                    mapFile = MapIOManager.Instance.GetBaseMap(entry.AssetPath);
                    mapFiles.Add(model.World, mapFile);
                }

                uint? worldAreaId = mapFile.GetWorldAreaId(new Vector3(model.X, model.Y, model.Z));
                if (!worldAreaId.HasValue)
                    continue;

                model.Area = (ushort)worldAreaId;

                log.Info($"Calculated area {worldAreaId} for entity {model.Id}.");
            }

            DatabaseManager.Instance.GetDatabase<WorldDatabase>().UpdateEntities(entities);

            log.Info($"Calculated area information for {entities.Count} {(entities.Count == 1 ? "entity" : "entities")}.");
        }

        /// <summary>
        /// Return <see cref="StatAttribute"/> for supplied <see cref="Stat"/>.
        /// </summary>
        public StatAttribute GetStatAttribute(Stat stat)
        {
            return statAttributes.TryGetValue(stat, out StatAttribute value) ? value : null;
        }
    }
}
