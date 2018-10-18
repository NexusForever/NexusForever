using System.Collections.Immutable;
using System.Linq;
using NexusForever.WorldServer.Database.World;
using NexusForever.WorldServer.Database.World.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public static class EntityManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public static ImmutableDictionary<uint, VendorInfo> VendorInfo { get; private set; }

        public static void Initialise()
        {
            InitialiseEntityVendorInfo();
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
    }
}
