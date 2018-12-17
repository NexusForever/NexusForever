using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NexusForever.WorldServer.Database.World.Model;

namespace NexusForever.WorldServer.Database.World
{
    public static class WorldDatabase
    {
        public static ImmutableList<Entity> GetEntities(ushort world)
        {
            using (var context = new WorldContext())
                return context.Entity
                    .Where(e => e.World == world)
                    .Include(c => c.EntityStat)
                    .Include(c => c.EntityProperty)
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public static ImmutableList<EntityVendor> GetEntityVendors()
        {
            using (var context = new WorldContext())
                return context.EntityVendor
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public static ImmutableList<EntityVendorCategory> GetEntityVendorCategories()
        {
            using (var context = new WorldContext())
                return context.EntityVendorCategory
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public static ImmutableList<EntityVendorItem> GetEntityVendorItems()
        {
            using (var context = new WorldContext())
                return context.EntityVendorItem
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public static ImmutableList<EntityStat> GetEntityStats()
        {
            using (var context = new WorldContext())
                return context.EntityStat
                    .AsNoTracking()
                    .ToImmutableList();
        }
        public static ImmutableList<EntityProperty> GetEntityProperties()
        {
            using (var context = new WorldContext())
                return context.EntityProperty
                    .AsNoTracking()
                    .ToImmutableList();
        }
    }
}
