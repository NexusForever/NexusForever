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
        public DbSet<EntityStatModel> EntityStat { get; set; }
        public DbSet<EntityVendorModel> EntityVendor { get; set; }
        public DbSet<EntityVendorCategoryModel> EntityVendorCategory { get; set; }
        public DbSet<EntityVendorItemModel> EntityVendorItem { get; set; }
        public DbSet<StoreCategoryModel> StoreCategory { get; set; }
        public DbSet<StoreOfferGroupModel> StoreOfferGroup { get; set; }
        public DbSet<StoreOfferGroupCategoryModel> StoreOfferGroupCategory { get; set; }
        public DbSet<StoreOfferItemModel> StoreOfferItem { get; set; }
        public DbSet<StoreOfferItemDataModel> StoreOfferItemData { get; set; }
        public DbSet<StoreOfferItemPriceModel> StoreOfferItemPrice { get; set; }
        public DbSet<ServerConfigModel> ServerConfig { get; set; }
        public DbSet<ServerConfigCharacterCreationLocationModel> ServerConfigCharacterCreationLocation { get; set; }
        public DbSet<TutorialModel> Tutorial { get; set; }

        private readonly IDatabaseConfig config;

        public WorldContext(IDatabaseConfig config)
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

                entity.HasKey(e => new { e.Type, e.ObjectId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ObjectId)
                    .HasColumnName("objectId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnName("note")
                    .HasColumnType("varchar(500)")
                    .HasDefaultValue("");
            });

            modelBuilder.Entity<EntityModel>(entity =>
            {
                entity.ToTable("entity");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ActivePropId)
                    .HasColumnName("activePropId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Area)
                    .HasColumnName("area")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Creature)
                    .HasColumnName("creature")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DisplayInfo)
                    .HasColumnName("displayInfo")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Faction1)
                    .HasColumnName("faction1")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Faction2)
                    .HasColumnName("faction2")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.OutfitInfo)
                    .HasColumnName("outfitInfo")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.QuestChecklistIdx)
                    .HasColumnName("questChecklistIdx")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Rx)
                    .HasColumnName("rx")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Ry)
                    .HasColumnName("ry")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Rz)
                    .HasColumnName("rz")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.World)
                    .HasColumnName("world")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.WorldSocketId)
                    .HasColumnName("worldSocketId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.X)
                    .HasColumnName("x")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Y)
                    .HasColumnName("y")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Z)
                    .HasColumnName("z")
                    .HasColumnType("float")
                    .HasDefaultValue(0);
            });

            modelBuilder.Entity<EntitySplineModel>(entity =>
            {
                entity.ToTable("entity_spline");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Fx)
                    .HasColumnName("fx")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Fy)
                    .HasColumnName("fy")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Fz)
                    .HasColumnName("fz")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Mode)
                    .HasColumnName("mode")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Speed)
                    .HasColumnName("speed")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.SplineId)
                    .HasColumnName("splineId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Entity)
                    .WithOne(p => p.EntitySpline)
                    .HasForeignKey<EntitySplineModel>(d => d.Id)
                    .HasConstraintName("FK__entity_spline_id__entity_id");
            });

            modelBuilder.Entity<EntityStatModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Stat })
                    .HasName("PRIMARY");

                entity.ToTable("entity_stats");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.EntityStat)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_stats_stat_id_entity_id");
            });

            modelBuilder.Entity<EntityVendorModel>(entity =>
            {
                entity.ToTable("entity_vendor");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.BuyPriceMultiplier)
                    .HasColumnName("buyPriceMultiplier")
                    .HasColumnType("float")
                    .HasDefaultValue(1f);

                entity.Property(e => e.SellPriceMultiplier)
                    .HasColumnName("sellPriceMultiplier")
                    .HasColumnType("float")
                    .HasDefaultValue(1f);

                entity.HasOne(d => d.Entity)
                    .WithOne(p => p.EntityVendor)
                    .HasForeignKey<EntityVendorModel>(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_id__entity_id");
            });

            modelBuilder.Entity<EntityVendorCategoryModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("entity_vendor_category");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.LocalisedTextId)
                    .HasColumnName("localisedTextId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.EntityVendorCategory)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_category_id__entity_id");
            });

            modelBuilder.Entity<EntityVendorItemModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("entity_vendor_item");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CategoryIndex)
                    .HasColumnName("categoryIndex")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Entity)
                    .WithMany(p => p.EntityVendorItem)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_item_id__entity_id");
            });

            modelBuilder.Entity<StoreCategoryModel>(entity =>
            {
                entity.ToTable("store_category");

                entity.HasIndex(e => e.ParentId)
                    .HasName("parentId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(150)")
                    .HasDefaultValue("");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(1);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValue("");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parentId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(26);

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasColumnType("tinyint(1) unsigned")
                    .HasDefaultValue(0);
            });

            modelBuilder.Entity<StoreOfferGroupModel>(entity =>
            {
                entity.ToTable("store_offer_group");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(500)")
                    .HasDefaultValue("");

                entity.Property(e => e.DisplayFlags)
                    .HasColumnName("displayFlags")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DisplayInfoOverride)
                    .HasColumnName("displayInfoOverride")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValue("");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasColumnType("tinyint(1) unsigned")
                    .HasDefaultValue(0);
            });

            modelBuilder.Entity<StoreOfferGroupCategoryModel>(entity =>
            {
                entity.ToTable("store_offer_group_category");

                entity.HasKey(e => new { e.Id, e.CategoryId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("FK__store_offer_group_category_categoryId__store_category_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasColumnType("tinyint(1) unsigned")
                    .HasDefaultValue(1);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.StoreOfferGroupCategory)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__store_offer_group_category_categoryId__store_category_id");

                entity.HasOne(d => d.OfferGroup)
                    .WithMany(p => p.StoreOfferGroupCategory)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__store_offer_group_category_id__store_offer_group_id");
            });

            modelBuilder.Entity<StoreOfferItemModel>(entity =>
            {
                entity.ToTable("store_offer_item");

                entity.HasKey(e => new { e.Id, e.GroupId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.GroupId)
                    .HasName("FK__store_offer_item_groupId__store_offer_group_id");

                entity.HasIndex(e => e.Id)
                    .HasName("id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.GroupId)
                    .HasColumnName("groupId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(500)")
                    .HasDefaultValue("");

                entity.Property(e => e.DisplayFlags)
                    .HasColumnName("displayFlags")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Field6)
                    .HasColumnName("field_6")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValue(0);

                entity.Property(e => e.Field7)
                    .HasColumnName("field_7")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValue("");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasColumnType("tinyint(1) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.StoreOfferItem)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK__store_offer_item_groupId__store_offer_group_id");
            });

            modelBuilder.Entity<StoreOfferItemDataModel>(entity =>
            {
                entity.ToTable("store_offer_item_data");

                entity.HasKey(e => new { e.Id, e.ItemId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(1);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.OfferItem)
                    .WithMany(p => p.StoreOfferItemData)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__store_offer_item_data_id__store_offer_item_id");
            });

            modelBuilder.Entity<StoreOfferItemPriceModel>(entity =>
            {
                entity.ToTable("store_offer_item_price");

                entity.HasKey(e => new { e.Id, e.CurrencyId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CurrencyId)
                    .HasColumnName("currencyId")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DiscountType)
                    .HasColumnName("discountType")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DiscountValue)
                    .HasColumnName("discountValue")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Expiry)
                    .HasColumnName("expiry")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValue(0);

                entity.Property(e => e.Field14)
                    .HasColumnName("field_14")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValue(0);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.OfferItem)
                    .WithMany(p => p.StoreOfferItemPrice)
                    .HasPrincipalKey(p => p.Id)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__store_offer_item_price_id__store_offer_item_id");
            });

            modelBuilder.Entity<ServerConfigModel>(entity =>
            {
                entity.ToTable("server_config");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

            });
            modelBuilder.Entity<ServerConfigCharacterCreationLocationModel>(entity =>
            {
                entity.ToTable("server_config_character_creation_location");

                entity.HasKey(e => new { e.Id, e.CharacterCreationId, e.RaceId, e.FactionId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CharacterCreationId)
                    .HasColumnName("characterCreationId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.RaceId)
                    .HasColumnName("raceId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.FactionId)
                    .HasColumnName("factionId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.StartingX)
                    .HasColumnName("locationX")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.StartingY)
                    .HasColumnName("locationY")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.StartingZ)
                    .HasColumnName("locationZ")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.RotationX)
                    .HasColumnName("rotationX")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.WorldId)
                    .HasColumnName("worldId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.WorldZoneId)
                    .HasColumnName("worldZoneId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(e => e.ServerConfig)
                    .WithMany(p => p.CharacterCreationLocations)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__server_config_config_id_creation_id");
            });

            modelBuilder.Entity<TutorialModel>(entity =>
            {
                entity.ToTable("tutorial");

                entity.HasKey(e => new { e.Id, e.Type, e.TriggerId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0)
                    .HasComment("Tutorial ID");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.TriggerId)
                    .HasColumnName("triggerId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnName("note")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValue("");
            });
        }
    }
}
