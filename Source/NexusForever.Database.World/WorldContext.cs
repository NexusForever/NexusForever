using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Configuration;
using NexusForever.Database.World.Model;

namespace NexusForever.Database.World
{
    public class WorldContext : DbContext
    {
        public DbSet<DisableModel> Disable { get; set; }
        public DbSet<EntityModel> Entity { get; set; }
        public DbSet<EntitySplineModel> EntitySpline { get; set; }
        public DbSet<EntityStatModel> EntityStats { get; set; }
        public DbSet<EntityVendorModel> EntityVendor { get; set; }
        public DbSet<EntityVendorCategoryModel> EntityVendorCategory { get; set; }
        public DbSet<EntityVendorItemModel> EntityVendorItem { get; set; }
        public DbSet<TutorialModel> Tutorial { get; set; }
        public DbSet<StoreCategoryModel> StoreCategory { get; set; }
        public DbSet<StoreOfferGroupModel> StoreOfferGroup { get; set; }
        public DbSet<StoreOfferGroupCategoryModel> StoreOfferGroupCategory { get; set; }
        public DbSet<StoreOfferItemModel> StoreOfferItem { get; set; }
        public DbSet<StoreOfferItemDataModel> StoreOfferItemData { get; set; }
        public DbSet<StoreOfferItemPriceModel> StoreOfferItemPrice { get; set; }

        private readonly IDatabaseConfiguration config;

        public WorldContext(IDatabaseConfiguration config)
        {
            this.config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(config, DatabaseType.World);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DisableModel>(entity =>
            {
                entity.ToTable("disable");

                entity.HasKey(e => new { e.Type, e.ObjectId });

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<EntityModel>(entity =>
            {
                entity.ToTable("entity");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ActivePropId)
                    .HasDefaultValue(0);

                entity.Property(e => e.QuestChecklistIdx)
                    .HasDefaultValue(0);
            });

            modelBuilder.Entity<EntitySplineModel>(entity =>
            {
                entity.ToTable("entity_spline");

                entity.HasOne(e => e.Entity)
                    .WithOne(e => e.Spline)
                    .HasForeignKey<EntitySplineModel>(e => e.Id);
            });

            modelBuilder.Entity<EntityStatModel>(entity =>
            {
                entity.ToTable("entity_stats");

                entity.HasKey(e => new { e.Id, e.Stat });

                entity.HasOne(e => e.Entity)
                    .WithMany(e => e.Stats)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<EntityVendorModel>(entity =>
            {
                entity.ToTable("entity_vendor");

                entity.HasOne(e => e.Entity)
                    .WithOne(e => e.Vendor)
                    .HasForeignKey<EntityVendorModel>(e => e.Id);

                entity.Property(e => e.BuyPriceMultiplier)
                    .HasDefaultValue(1);

                entity.Property(e => e.SellPriceMultiplier)
                    .HasDefaultValue(1);
            });

            modelBuilder.Entity<EntityVendorCategoryModel>(entity =>
            {
                entity.ToTable("entity_vendor_category");

                entity.HasKey(e => new { e.Id, e.Index });

                entity.HasOne(e => e.Entity)
                    .WithMany(e => e.VendorCategories)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<EntityVendorItemModel>(entity =>
            {
                entity.ToTable("entity_vendor_item");

                entity.HasKey(e => new { e.Id, e.Index });

                entity.HasOne(e => e.Entity)
                    .WithMany(e => e.VendorItems)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<StoreCategoryModel>(entity =>
            {
                entity.ToTable("store_category");

                entity.HasIndex(e => e.ParentId);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(150)");

                entity.Property(e => e.Index)
                    .HasDefaultValue(1);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.ParentId)
                    .HasDefaultValue(26);
            });

            modelBuilder.Entity<StoreOfferGroupModel>(entity =>
            {
                entity.ToTable("store_offer_group");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<StoreOfferGroupCategoryModel>(entity =>
            {
                entity.ToTable("store_offer_group_category");

                entity.HasKey(e => new { e.Id, e.CategoryId });

                entity.HasIndex(e => e.CategoryId);

                entity.HasOne(e => e.OfferGroup)
                    .WithMany(e => e.Categories)
                    .HasForeignKey(e => e.Id);

                entity.HasOne(e => e.Category)
                    .WithMany(e => e.StoreOfferGroupCategory)
                    .HasForeignKey(e => e.CategoryId);

                entity.Property(e => e.Visible)
                    .HasDefaultValue(1);
            });

            modelBuilder.Entity<StoreOfferItemModel>(entity =>
            {
                entity.ToTable("store_offer_item");

                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.OfferGroup)
                    .WithMany(e => e.Items)
                    .HasForeignKey(e => e.GroupId);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<StoreOfferItemDataModel>(entity =>
            {
                entity.ToTable("store_offer_item_data");

                entity.HasKey(e => new { e.Id, e.ItemId });

                entity.HasOne(e => e.Item)
                    .WithMany(e => e.Items)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(e => e.Id);

                entity.Property(e => e.Amount)
                    .HasDefaultValue(1);
            });

            modelBuilder.Entity<StoreOfferItemPriceModel>(entity =>
            {
                entity.ToTable("store_offer_item_price");

                entity.HasKey(e => new { e.Id, e.CurrencyId });

                entity.HasOne(e => e.Item)
                    .WithMany(e => e.Prices)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<TutorialModel>(entity =>
            {
                entity.ToTable("tutorial");

                entity.HasKey(e => new { e.Id, e.Type, e.TriggerId });

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnType("varchar(50)");
            });
        }
    }
}
