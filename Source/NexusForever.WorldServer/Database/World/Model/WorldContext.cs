using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class WorldContext : DbContext
    {
        public WorldContext()
        {
        }

        public WorldContext(DbContextOptions<WorldContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Disable> Disable { get; set; }
        public virtual DbSet<Entity> Entity { get; set; }
        public virtual DbSet<EntitySpline> EntitySpline { get; set; }
        public virtual DbSet<EntityStats> EntityStats { get; set; }
        public virtual DbSet<EntityVendor> EntityVendor { get; set; }
        public virtual DbSet<EntityVendorCategory> EntityVendorCategory { get; set; }
        public virtual DbSet<EntityVendorItem> EntityVendorItem { get; set; }
        public virtual DbSet<Tutorial> Tutorial { get; set; }
        public virtual DbSet<StoreCategory> StoreCategory { get; set; }
        public virtual DbSet<StoreOfferGroup> StoreOfferGroup { get; set; }
        public virtual DbSet<StoreOfferGroupCategory> StoreOfferGroupCategory { get; set; }
        public virtual DbSet<StoreOfferItem> StoreOfferItem { get; set; }
        public virtual DbSet<StoreOfferItemData> StoreOfferItemData { get; set; }
        public virtual DbSet<StoreOfferItemPrice> StoreOfferItemPrice { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(DatabaseManager.Config, DatabaseType.World);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Disable>(entity =>
            {
                entity.HasKey(e => new { e.Type, e.ObjectId })
                    .HasName("PRIMARY");

                entity.ToTable("disable");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ObjectId)
                    .HasColumnName("objectId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnName("note")
                    .HasColumnType("varchar(500)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.ToTable("entity");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ActivePropId)
                    .HasColumnName("activePropId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Area)
                    .HasColumnName("area")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Creature)
                    .HasColumnName("creature")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DisplayInfo)
                    .HasColumnName("displayInfo")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Faction1)
                    .HasColumnName("faction1")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Faction2)
                    .HasColumnName("faction2")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.OutfitInfo)
                    .HasColumnName("outfitInfo")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.QuestChecklistIdx)
                    .HasColumnName("questChecklistIdx")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Rx)
                    .HasColumnName("rx")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Ry)
                    .HasColumnName("ry")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Rz)
                    .HasColumnName("rz")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.World)
                    .HasColumnName("world")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.X)
                    .HasColumnName("x")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Y)
                    .HasColumnName("y")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Z)
                    .HasColumnName("z")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<EntitySpline>(entity =>
            {
                entity.ToTable("entity_spline");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Fx)
                    .HasColumnName("fx")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Fy)
                    .HasColumnName("fy")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Fz)
                    .HasColumnName("fz")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Mode)
                    .HasColumnName("mode")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Speed)
                    .HasColumnName("speed")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SplineId)
                    .HasColumnName("splineId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.EntitySpline)
                    .HasForeignKey<EntitySpline>(d => d.Id)
                    .HasConstraintName("FK__entity_spline_id__entity_id");
            });

            modelBuilder.Entity<EntityStats>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Stat })
                    .HasName("PRIMARY");

                entity.ToTable("entity_stats");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.EntityStats)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_stats_stat_id_entity_id");
            });

            modelBuilder.Entity<EntityVendor>(entity =>
            {
                entity.ToTable("entity_vendor");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BuyPriceMultiplier)
                    .HasColumnName("buyPriceMultiplier")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.SellPriceMultiplier)
                    .HasColumnName("sellPriceMultiplier")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.EntityVendor)
                    .HasForeignKey<EntityVendor>(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_id__entity_id");
            });

            modelBuilder.Entity<EntityVendorCategory>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("entity_vendor_category");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LocalisedTextId)
                    .HasColumnName("localisedTextId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.EntityVendorCategory)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_category_id__entity_id");
            });

            modelBuilder.Entity<EntityVendorItem>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("entity_vendor_item");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CategoryIndex)
                    .HasColumnName("categoryIndex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.EntityVendorItem)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_item_id__entity_id");
            });

            modelBuilder.Entity<StoreCategory>(entity =>
            {
                entity.ToTable("store_category");

                entity.HasIndex(e => e.ParentId)
                    .HasName("parentId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(150)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parentId")
                    .HasDefaultValueSql("'26'");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<StoreOfferGroup>(entity =>
            {
                entity.ToTable("store_offer_group");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(500)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DisplayFlags)
                    .HasColumnName("displayFlags")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field2)
                    .HasColumnName("field_2")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<StoreOfferGroupCategory>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CategoryId })
                    .HasName("PRIMARY");

                entity.ToTable("store_offer_group_category");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("FK__store_offer_group_category_categoryId__store_category_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.Index).HasColumnName("index");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.StoreOfferGroupCategory)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__store_offer_group_category_categoryId__store_category_id");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.StoreOfferGroupCategory)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__store_offer_group_category_id__store_offer_group_id");
            });

            modelBuilder.Entity<StoreOfferItem>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.GroupId })
                    .HasName("PRIMARY");

                entity.ToTable("store_offer_item");

                entity.HasIndex(e => e.GroupId)
                    .HasName("FK__store_offer_item_groupId__store_offer_group_id");

                entity.HasIndex(e => e.Id)
                    .HasName("id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.GroupId)
                    .HasColumnName("groupId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(500)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DisplayFlags)
                    .HasColumnName("displayFlags")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field6)
                    .HasColumnName("field_6")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field7)
                    .HasColumnName("field_7")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.StoreOfferItem)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK__store_offer_item_groupId__store_offer_group_id");
            });

            modelBuilder.Entity<StoreOfferItemData>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.ItemId })
                    .HasName("PRIMARY");

                entity.ToTable("store_offer_item_data");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.StoreOfferItemData)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__store_offer_item_data_id__store_offer_item_id");
            });

            modelBuilder.Entity<StoreOfferItemPrice>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CurrencyId })
                    .HasName("PRIMARY");

                entity.ToTable("store_offer_item_price");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CurrencyId)
                    .HasColumnName("currencyId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DiscountType)
                    .HasColumnName("discountType")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DiscountValue)
                    .HasColumnName("discountValue")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Expiry)
                    .HasColumnName("expiry")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field14)
                    .HasColumnName("field_14")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.StoreOfferItemPrice)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__store_offer_item_price_id__store_offer_item_id");
            });

            modelBuilder.Entity<Tutorial>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.TriggerId })
                    .HasName("PRIMARY");

                entity.ToTable("tutorial");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TriggerId)
                    .HasColumnName("triggerId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnName("note")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");
            });
        }
    }
}
