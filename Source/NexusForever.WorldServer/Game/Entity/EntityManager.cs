using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.IO.Map;
using NexusForever.WorldServer.Database.World;
using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Map;
using NLog;
using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;

namespace NexusForever.WorldServer.Game.Entity
{
    public static class EntityManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private delegate IDatabaseEntity DatabaseEntityFactoryDelegate();
        private static ImmutableDictionary<EntityType, DatabaseEntityFactoryDelegate> databaseEntityFactories;

        public static ImmutableDictionary<uint, VendorInfo> VendorInfo { get; private set; }
        public static ImmutableDictionary<uint, EntityStat> EntityStat { get; private set; }


        public static void Initialise()
        {
            InitialiseEntityFactories();
            InitialiseEntityVendorInfo();
            InitialiseEntityStats();

            CalculateEntityAreaData();
        }

        private static void InitialiseEntityFactories()
        {
            var builder = ImmutableDictionary.CreateBuilder<EntityType, DatabaseEntityFactoryDelegate>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                DatabaseEntity attribute = type.GetCustomAttribute<DatabaseEntity>();
                if (attribute == null)
                    continue;

                ConstructorInfo constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

                NewExpression @new = Expression.New(constructor);
                builder.Add(attribute.EntityType, Expression.Lambda<DatabaseEntityFactoryDelegate>(@new).Compile());
            }

            databaseEntityFactories = builder.ToImmutable();
        }

        private static void InitialiseEntityVendorInfo()
        {
            ImmutableDictionary<uint, EntityVendor> vendors = WorldDatabase.GetEntityVendors()
                .GroupBy(v => v.Id)
                .ToImmutableDictionary(g => g.Key, g => g.First());

            ImmutableDictionary<uint, ImmutableList<EntityVendorCategory>> vendorCategories = WorldDatabase.GetEntityVendorCategories()
                .GroupBy(c => c.Id)
                .ToImmutableDictionary(g => g.Key, g => g.ToImmutableList());

            ImmutableDictionary<uint, ImmutableList<EntityVendorItem>> vendorItems = WorldDatabase.GetEntityVendorItems()
                .GroupBy(i => i.Id)
                .ToImmutableDictionary(g => g.Key, g => g.ToImmutableList());

            // category with no items
            foreach (uint source in vendorCategories.Keys.Except(vendorItems.Keys))
            {
                
            }

            // items with no category
            foreach (uint source in vendorItems.Keys.Except(vendorCategories.Keys))
            {

            }

            VendorInfo = vendorCategories.Keys
                .Select(i => new VendorInfo(vendors[i], vendorCategories[i], vendorItems[i]))
                .ToImmutableDictionary(v => v.Id, v => v);

            log.Info($"Loaded vendor information for {VendorInfo.Count} {(VendorInfo.Count > 1 ? "entities" : "entity")}.");
        }

        private static void InitialiseEntityStats()
        {
            ImmutableDictionary<uint, EntityStat> stats = WorldDatabase.GetEntityStats()
                .GroupBy(v => v.Id)
                .ToImmutableDictionary(g => g.Key, g => g.First());
        }

        [Conditional("DEBUG")]
        private static void CalculateEntityAreaData()
        {
            log.Info("Calculating area information for entities...");

            var mapFiles = new Dictionary<ushort, MapFile>();
            var entities = new HashSet<EntityModel>();

            foreach (EntityModel model in WorldDatabase.GetEntitiesWithoutArea())
            {
                entities.Add(model);

                if (!mapFiles.TryGetValue(model.World, out MapFile mapFile))
                {
                    WorldEntry entry = GameTableManager.World.GetEntry(model.World);
                    mapFile = BaseMap.LoadMapFile(entry.AssetPath);
                    mapFiles.Add(model.World, mapFile);
                }

                uint worldAreaId = mapFile.GetWorldAreaId(new Vector3(model.X, model.Y, model.Z));
                model.Area = (ushort)worldAreaId;

                log.Info($"Calculated area {worldAreaId} for entity {model.Id}.");
            }

            WorldDatabase.UpdateEntities(entities);

            log.Info($"Calculated area information for {entities.Count} {(entities.Count == 1 ? "entity" : "entities")}.");
        }

        /// <summary>
        /// Return a new <see cref="IDatabaseEntity"/> for supplied <see cref="EntityType"/>.
        /// </summary>
        public static IDatabaseEntity NewEntity(EntityType type)
        {
            return databaseEntityFactories.TryGetValue(type, out DatabaseEntityFactoryDelegate factory) ? factory.Invoke() : null;
        }
    }
}
