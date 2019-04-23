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

        public virtual DbSet<Entity> Entity { get; set; }
        public virtual DbSet<EntityVendor> EntityVendor { get; set; }
        public virtual DbSet<EntityVendorCategory> EntityVendorCategory { get; set; }
        public virtual DbSet<EntityVendorItem> EntityVendorItem { get; set; }
        public virtual DbSet<EntityStat> EntityStat { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(DatabaseManager.Config, DatabaseType.World);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>(entity =>
            {
                entity.ToTable("entity");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
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

                entity.Property(e => e.QuestChecklistIdx)
                    .HasColumnName("questChecklistIdx")
                    .HasDefaultValueSql("'0'");
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
                entity.HasKey(e => new { e.Id, e.Index });

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
                entity.HasKey(e => new { e.Id, e.Index });

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

            modelBuilder.Entity<EntityStat>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Stat });

                entity.ToTable("entity_stats");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasDefaultValueSql("'0'")
                    .ValueGeneratedNever();

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.EntityStat)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_stats_stat_id_entity_id");
            });
        }
    }
}
