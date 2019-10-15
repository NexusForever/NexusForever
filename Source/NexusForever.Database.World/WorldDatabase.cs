using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NexusForever.Database.Configuration;
using NexusForever.Database.World.Model;
using NLog;

namespace NexusForever.Database.World
{
    public class WorldDatabase
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly IDatabaseConfiguration config;

        public WorldDatabase(IDatabaseConfiguration config)
        {
            this.config = config;
        }

        public void Migrate()
        {
            using (var context = new WorldContext(config))
            {
                List<string> migrations = context.Database.GetPendingMigrations().ToList();
                if (migrations.Count > 0)
                {
                    log.Info($"Applying {migrations.Count} world database migration(s)...");
                    foreach (string migration in migrations)
                        log.Info(migration);

                    context.Database.Migrate();
                }
            }
        }

        public ImmutableList<EntityModel> GetEntities(ushort world)
        {
            using (var context = new WorldContext(config))
                return context.Entity.Where(e => e.World == world)
                    .Include(e => e.Vendor)
                    .Include(e => e.VendorCategories)
                    .Include(e => e.VendorItems)
                    .Include(e => e.Stats)
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public ImmutableList<EntityModel> GetEntitiesWithoutArea()
        {
            using (var context = new WorldContext(config))
                return context.Entity.Where(e => e.Area == 0)
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public void UpdateEntities(IEnumerable<EntityModel> models)
        {
            using (var context = new WorldContext(config))
            {
                foreach (EntityModel model in models)
                {
                    EntityEntry<EntityModel> entity = context.Attach(model);
                    entity.State = EntityState.Modified;
                }
               
                context.SaveChanges();
            }
        }

        public ImmutableList<TutorialModel> GetTutorialTriggers()
        {
            using (var context = new WorldContext(config))
                return context.Tutorial.ToImmutableList();
        }

        public ImmutableList<DisableModel> GetDisables()
        {
            using (var context = new WorldContext(config))
                return context.Disable.ToImmutableList();
        }

        public ImmutableList<StoreCategoryModel> GetStoreCategories()
        {
            using (var context = new WorldContext(config))
                return context.StoreCategory
                    .AsNoTracking()
                    .ToImmutableList();
        }

        public ImmutableList<StoreOfferGroupModel> GetStoreOfferGroups()
        {
            using (var context = new WorldContext(config))
                return context.StoreOfferGroup
                    .Include(e => e.Categories)
                    .Include(e => e.Items)
                        .ThenInclude(e => e.Items)
                    .Include(e => e.Items)
                        .ThenInclude(e => e.Prices)
                    .AsNoTracking()
                    .ToImmutableList();
        }
    }
}
