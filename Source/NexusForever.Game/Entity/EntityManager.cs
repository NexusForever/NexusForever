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
using NexusForever.IO.Format.NexusForever.Map;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Entity
{
    public sealed class EntityManager : Singleton<EntityManager>, IEntityManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ImmutableDictionary<Static.Entity.Stat, StatAttribute> statAttributes;

        public delegate void VitalSetHandler(WorldEntity instance, float value);
        public delegate float VitalGetHandler(WorldEntity instance);
        private ImmutableDictionary<Vital, VitalSetHandler> vitalSetters;
        private ImmutableDictionary<Vital, VitalGetHandler> vitalGetters;

        public void Initialise()
        {
            InitialiseEntityStats();
            InitialiseEntityVitals();

            CalculateEntityAreaData();
        }

        private void InitialiseEntityStats()
        {
            var builder = ImmutableDictionary.CreateBuilder<Static.Entity.Stat, StatAttribute>();

            foreach (FieldInfo field in typeof(Static.Entity.Stat).GetFields())
            {
                StatAttribute attribute = field.GetCustomAttribute<StatAttribute>();
                if (attribute == null)
                    continue;

                var stat = (Static.Entity.Stat)field.GetValue(null);
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

        private void InitialiseEntityVitals()
        {
            var setterBuilder = ImmutableDictionary.CreateBuilder<Vital, VitalSetHandler>();
            var getterBuilder = ImmutableDictionary.CreateBuilder<Vital, VitalGetHandler>();

            foreach (PropertyInfo property in typeof(WorldEntity).GetProperties())
            {
                IEnumerable<VitalAttribute> vitalAttributes = property.GetCustomAttributes<VitalAttribute>();
                if (vitalAttributes.Count() == 0)
                    continue;

                VitalSetHandler vitalSetterDelegate = (VitalSetHandler)Delegate.CreateDelegate(typeof(VitalSetHandler), null, property.GetSetMethod());
                VitalGetHandler vitalGetterDelegate = (VitalGetHandler)Delegate.CreateDelegate(typeof(VitalGetHandler), null, property.GetGetMethod());

                foreach (VitalAttribute attribute in vitalAttributes)
                {
                    setterBuilder.Add(attribute.Vital, vitalSetterDelegate);
                    getterBuilder.Add(attribute.Vital, vitalGetterDelegate);
                }
            }

            vitalSetters = setterBuilder.ToImmutable();
            vitalGetters = getterBuilder.ToImmutable();
        }

        /// <summary>
        /// Return <see cref="StatAttribute"/> for supplied <see cref="Stat"/>.
        /// </summary>
        public StatAttribute GetStatAttribute(Static.Entity.Stat stat)
        {
            return statAttributes.TryGetValue(stat, out StatAttribute value) ? value : null;
        }

        /// <summary>
        /// Return <see cref="VitalSetHandler"/> for supplied <see cref="Vital"/>.
        /// </summary>
        public VitalSetHandler GetVitalSetter(Vital vital)
        {
            if (!vitalSetters.TryGetValue(vital, out VitalSetHandler vitalProp))
            {
                log.Trace($"Unhandled Vital: {vital}");
                return null;
            }

            return vitalProp;
        }

        /// <summary>
        /// Return <see cref="VitalSetHandler"/> for supplied <see cref="Vital"/>.
        /// </summary>
        public VitalGetHandler GetVitalGetter(Vital vital)
        {
            if (!vitalGetters.TryGetValue(vital, out VitalGetHandler vitalProp))
            {
                log.Trace($"Unhandled Vital: {vital}");
                return null;
            }

            return vitalProp;
        }
    }
}
