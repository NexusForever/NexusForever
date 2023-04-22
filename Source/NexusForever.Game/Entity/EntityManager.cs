﻿using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using NexusForever.Database.World;
using NexusForever.Database;
using NexusForever.Database.World.Model;
using NexusForever.Game.Map;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.IO.Map;
using NexusForever.Shared;
using NLog;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Entity
{
    public sealed class EntityManager : Singleton<EntityManager>, IEntityManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private delegate WorldEntity EntityFactoryDelegate();
        private ImmutableDictionary<EntityType, EntityFactoryDelegate> entityFactories;

        private ImmutableDictionary<Stat, StatAttribute> statAttributes;

        public void Initialise()
        {
            InitialiseEntityFactories();
            InitialiseEntityStats();

            CalculateEntityAreaData();
        }

        private void InitialiseEntityFactories()
        {
            var builder = ImmutableDictionary.CreateBuilder<EntityType, EntityFactoryDelegate>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                DatabaseEntityAttribute attribute = type.GetCustomAttribute<DatabaseEntityAttribute>();
                if (attribute == null)
                    continue;

                ConstructorInfo constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

                NewExpression @new = Expression.New(constructor);
                builder.Add(attribute.EntityType, Expression.Lambda<EntityFactoryDelegate>(@new).Compile());
            }

            entityFactories = builder.ToImmutable();
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
        /// Return a new <see cref="IWorldEntity"/> of supplied <see cref="EntityType"/>.
        /// </summary>
        public IWorldEntity NewEntity(EntityType type)
        {
            return entityFactories.TryGetValue(type, out EntityFactoryDelegate factory) ? factory.Invoke() : null;
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
