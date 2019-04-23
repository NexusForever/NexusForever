using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.WorldServer.Database.World.Model;

namespace NexusForever.WorldServer.Database.World
{
    public static class WorldDatabase
    {
        public static ImmutableList<Entity> GetEntities(ushort world)
        {
            using (var context = new WorldContext())
                return context.Entity.Where(e => e.World == world)
                    .Include(e => e.EntityVendor)
                    .Include(e => e.EntityVendorCategory)
                    .Include(e => e.EntityVendorItem)
                    .Include(e => e.EntityStat)
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public static ImmutableList<Entity> GetEntitiesWithoutArea()
        {
            using (var context = new WorldContext())
                return context.Entity.Where(e => e.Area == 0)
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public static void UpdateEntities(IEnumerable<Entity> models)
        {
            using (var context = new WorldContext())
            {
                foreach (Entity model in models)
                {
                    EntityEntry<Entity> entity = context.Attach(model);
                    entity.State = EntityState.Modified;
                }
               
                context.SaveChanges();
            }
        }
    }
}
