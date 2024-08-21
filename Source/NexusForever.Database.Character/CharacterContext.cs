using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Character.Model;
using NexusForever.Database.Configuration.Model;

namespace NexusForever.Database.Character
{
    public class CharacterContext : DbContext
    {
        public DbSet<CharacterModel> Character { get; set; }
        public DbSet<CharacterAchievementModel> CharacterAchievement { get; set; }
        public DbSet<CharacterActionSetAmpModel> CharacterActionSetAmp { get; set; }
        public DbSet<CharacterActionSetShortcutModel> CharacterActionSetShortcut { get; set; }
        public DbSet<CharacterAppearanceModel> CharacterAppearance { get; set; }
        public DbSet<CharacterBoneModel> CharacterBone { get; set; }
        public DbSet<CharacterCostumeModel> CharacterCostume { get; set; }
        public DbSet<CharacterCostumeItemModel> CharacterCostumeItem { get; set; }
        public DbSet<CharacterCreateModel> CharacterCreate { get; set; }
        public DbSet<CharacterCurrencyModel> CharacterCurrency { get; set; }
        public DbSet<CharacterCustomisationModel> CharacterCustomisation { get; set; }
        public DbSet<CharacterDatacubeModel> CharacterDatacube { get; set; }
        public DbSet<CharacterEntitlementModel> CharacterEntitlement { get; set; }
        public DbSet<CharacterKeybindingModel> CharacterKeybinding { get; set; }
        public DbSet<CharacterMailModel> CharacterMail { get; set; }
        public DbSet<CharacterMailAttachmentModel> CharacterMailAttachment { get; set; }
        public DbSet<CharacterPathModel> CharacterPath { get; set; }
        public DbSet<CharacterPetCustomisationModel> CharacterPetCustomisation { get; set; }
        public DbSet<CharacterPetFlairModel> CharacterPetFlair { get; set; }
        public DbSet<CharacterQuestModel> CharacterQuest { get; set; }
        public DbSet<CharacterQuestObjectiveModel> CharacterQuestObjective { get; set; }
        public DbSet<CharacterReputation> CharacterReputation { get; set; }
        public DbSet<CharacterSpellModel> CharacterSpell { get; set; }
        public DbSet<CharacterStatModel> CharacterStat { get; set; }
        public DbSet<CharacterTitleModel> CharacterTitle { get; set; }
        public DbSet<CharacterTradeskillMaterialModel> CharacterTradeskillMaterial { get; set; }
        public DbSet<CharacterZonemapHexgroupModel> CharacterZonemapHexgroup { get; set; }
        public DbSet<ChatChannelModel> ChatChannel { get; set; }
        public DbSet<ChatChannelMemberModel> ChatChannelMember { get; set; }
        public DbSet<GuildModel> Guild { get; set; }
        public DbSet<GuildRankModel> GuildRank { get; set; }
        public DbSet<GuildMemberModel> GuildMember { get; set; }
        public DbSet<GuildAchievementModel> GuildAchievement { get; set; }
        public DbSet<GuildDataModel> GuildData { get; set; }
        public DbSet<ItemModel> Item { get; set; }
        public DbSet<PropertyBaseModel> PropertyBase { get; set; }
        public DbSet<ResidenceModel> Residence { get; set; }
        public DbSet<ResidenceDecor> ResidenceDecor { get; set; }
        public DbSet<ResidencePlotModel> ResidencePlot { get; set; }

        private readonly IConnectionString config;

        public CharacterContext(IConnectionString config)
        {
            this.config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(config);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterModel>(entity =>
            {
                entity.ToTable("character");

                entity.HasIndex(e => e.AccountId)
                    .HasDatabaseName("accountId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.AccountId)
                    .HasColumnName("accountId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ActiveCostumeIndex)
                    .HasColumnName("activeCostumeIndex")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValue(-1);

                entity.Property(e => e.ActivePath)
                    .HasColumnName("activePath")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ActiveSpec)
                    .HasColumnName("activeSpec")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Class)
                    .HasColumnName("class")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteTime)
                    .HasColumnName("deleteTime")
                    .HasColumnType("datetime")
                    .HasDefaultValue(null);

                entity.Property(e => e.FactionId)
                    .HasColumnName("factionId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.GuildAffiliation)
                    .HasColumnName("guildAffiliation");

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasDefaultValue(0);

                entity.Property(e => e.InnateIndex)
                    .HasColumnName("innateIndex")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.InputKeySet)
                    .HasColumnName("inputKeySet")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValue(0);

                entity.Property(e => e.LastOnline)
                    .HasColumnName("lastOnline")
                    .HasColumnType("datetime");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.LocationX)
                    .HasColumnName("locationX")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.LocationY)
                    .HasColumnName("locationY")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.LocationZ)
                    .HasColumnName("locationZ")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.RotationX)
                    .HasColumnName("rotationX")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.RotationY)
                    .HasColumnName("rotationY")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.RotationZ)
                    .HasColumnName("rotationZ")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValue("");

                entity.Property(e => e.OriginalName)
                    .HasColumnName("originalName")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValue(null);

                entity.Property(e => e.PathActivatedTimestamp)
                    .HasColumnName("pathActivatedTimestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Race)
                    .HasColumnName("race")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.RestBonusXp)
                    .HasColumnName("restBonusXp")
                    .HasColumnType("int unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Sex)
                    .HasColumnName("sex")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.TimePlayedLevel)
                    .HasColumnName("timePlayedLevel")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.TimePlayedTotal)
                    .HasColumnName("timePlayedTotal")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.TotalXp)
                    .HasColumnName("totalXp")
                    .HasColumnType("int unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.WorldId)
                    .HasColumnName("worldId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.WorldZoneId)
                    .HasColumnName("worldZoneId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);
            });

            modelBuilder.Entity<CharacterAchievementModel>(entity =>
            {
                entity.ToTable("character_achievement");

                entity.HasKey(e => new { e.Id, e.AchievementId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.AchievementId)
                    .HasColumnName("achievementId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Data0)
                    .HasColumnName("data0")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Data1)
                    .HasColumnName("data1")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DateCompleted)
                    .HasColumnName("dateCompleted")
                    .HasColumnType("datetime")
                    .HasDefaultValue(null);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Achievement)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_achievement_id__character_id");
            });

            modelBuilder.Entity<CharacterActionSetAmpModel>(entity =>
            {
                entity.ToTable("character_action_set_amp");

                entity.HasKey(e => new { e.Id, e.SpecIndex, e.AmpId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.SpecIndex)
                    .HasColumnName("specIndex")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.AmpId)
                    .HasColumnName("ampId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.ActionSetAmp)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_action_set_amp_id__character_id");
            });

            modelBuilder.Entity<CharacterActionSetShortcutModel>(entity =>
            {
                entity.ToTable("character_action_set_shortcut");

                entity.HasKey(e => new { e.Id, e.SpecIndex, e.Location })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.SpecIndex)
                    .HasColumnName("specIndex")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.ObjectId)
                    .HasColumnName("objectId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ShortcutType)
                    .HasColumnName("shortcutType")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Tier)
                    .HasColumnName("tier")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.ActionSetShortcut)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_action_set_shortcut_id__character_id");
            });

            modelBuilder.Entity<CharacterAppearanceModel>(entity =>
            {
                entity.ToTable("character_appearance");

                entity.HasKey(e => new { e.Id, e.Slot })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Slot)
                    .HasColumnName("slot")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DisplayId)
                    .HasColumnName("displayId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Appearance)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_appearance_id__character_id");
            });

            modelBuilder.Entity<CharacterBoneModel>(entity =>
            {
                entity.ToTable("character_bone");

                entity.HasKey(e => new { e.Id, e.BoneIndex })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.BoneIndex)
                    .HasColumnName("boneIndex")
                    .HasColumnType("tinyint(4) unsigned");

                entity.Property(e => e.Bone)
                    .HasColumnName("bone")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Bone)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK_character_bone_id__character_id");
            });

            modelBuilder.Entity<CharacterCostumeModel>(entity =>
            {
                entity.ToTable("character_costume");

                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Mask)
                    .HasColumnName("mask")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Costume)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_costume_id__character_id");
            });

            modelBuilder.Entity<CharacterCostumeItemModel>(entity =>
            {
                entity.ToTable("character_costume_item");

                entity.HasKey(e => new { e.Id, e.Index, e.Slot })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Slot)
                    .HasColumnName("slot")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.DyeData)
                    .HasColumnName("dyeData")
                    .HasColumnType("int(10)")
                    .HasDefaultValue(0);

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Costume)
                    .WithMany(p => p.CostumeItem)
                    .HasForeignKey(d => new { d.Id, d.Index })
                    .HasConstraintName("FK__character_costume_item_id-index__character_costume_id-index");
            });

            modelBuilder.Entity<CharacterCreateModel>(entity =>
            {
                entity.ToTable("character_create");

                entity.HasKey(e => new { e.Race, e.Faction, e.CreationStart })
                    .HasName("PRIMARY");

                entity.Property(e => e.Race)
                    .HasColumnName("race")
                    .HasColumnType("tinyint(4) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Faction)
                    .HasColumnName("faction")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CreationStart)
                    .HasColumnName("creationStart")
                    .HasColumnType("tinyint(4) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.WorldId)
                    .HasColumnName("worldId")
                    .HasColumnType("int(10) unsigned")
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

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(200)")
                    .HasDefaultValue("");

                entity.HasData(
                    new CharacterCreateModel
                    {
                        Race          = 1,
                        Faction       = 167,
                        CreationStart = 4,
                        WorldId       = 3460,
                        X             = 29.1286f,
                        Y             = -853.8716f,
                        Z             = -560.188f,
                        Rx            = -2.751458f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Human - Novice"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 1,
                        Faction       = 167,
                        CreationStart = 3,
                        WorldId       = 426,
                        X             = 4110.71f,
                        Y             = -658.6249f,
                        Z             = -5145.48f,
                        Rx            = 0.317613f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Human - Veteran"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 1,
                        Faction       = 167,
                        CreationStart = 5,
                        WorldId       = 51,
                        X             = 4074.34f,
                        Y             = -797.8368f,
                        Z             = -2399.37f,
                        Rx            = 0f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Human - Level 50"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 3,
                        Faction       = 167,
                        CreationStart = 4,
                        WorldId       = 3460,
                        X             = 29.1286f,
                        Y             = -853.8716f,
                        Z             = -560.188f,
                        Rx            = -2.751458f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Granok - Novice"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 3,
                        Faction       = 167,
                        CreationStart = 3,
                        WorldId       = 426,
                        X             = 4110.71f,
                        Y             = -658.6249f,
                        Z             = -5145.48f,
                        Rx            = 0.317613f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment = "Exile Granok- Veteran"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 3,
                        Faction       = 167,
                        CreationStart = 5,
                        WorldId       = 51,
                        X             = 4074.34f,
                        Y             = -797.8368f,
                        Z             = -2399.37f,
                        Rx            = 0f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Granok - Level 50"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 4,
                        Faction       = 167,
                        CreationStart = 4,
                        WorldId       = 3460,
                        X             = 29.1286f,
                        Y             = -853.8716f,
                        Z             = -560.188f,
                        Rx            = -2.751458f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Aurin - Novice"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 4,
                        Faction       = 167,
                        CreationStart = 3,
                        WorldId       = 990,
                        X             = -771.823f,
                        Y             = -904.2852f,
                        Z             = -2269.56f,
                        Rx            = -1.1214035f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Aurin - Veteran"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 4,
                        Faction       = 167,
                        CreationStart = 5,
                        WorldId       = 51,
                        X             = 4074.34f,
                        Y             = -797.8368f,
                        Z             = -2399.37f,
                        Rx            = 0f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Aurin - Level 50"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 16,
                        Faction       = 167,
                        CreationStart = 4,
                        WorldId       = 3460,
                        X             = 29.1286f,
                        Y             = -853.8716f,
                        Z             = -560.188f,
                        Rx            = -2.751458f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Mordesh - Novice"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 16,
                        Faction       = 167,
                        CreationStart = 3,
                        WorldId       = 990,
                        X             = -771.823f,
                        Y             = -904.2852f,
                        Z             = -2269.56f,
                        Rx            = -1.1214035f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Mordesh - Veteran"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 16,
                        Faction       = 167,
                        CreationStart = 5,
                        WorldId       = 51,
                        X             = 4074.34f,
                        Y             = -797.8368f,
                        Z             = -2399.37f,
                        Rx            = 0f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Exile Mordesh - Level 50"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 13,
                        Faction       = 166,
                        CreationStart = 4,
                        WorldId       = 3460,
                        X             = 29.1286f,
                        Y             = -853.8716f,
                        Z             = -560.188f,
                        Rx            = -2.751458f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Chua - Novice"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 13,
                        Faction       = 166,
                        CreationStart = 3,
                        WorldId       = 870,
                        X             = -8261.3984f,
                        Y             = -995.471f,
                        Z             = -242.3648f,
                        Rx            = -2.215535f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Chua - Veteran"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 13,
                        Faction       = 166,
                        CreationStart = 5,
                        WorldId       = 22,
                        X             = -3343.58f,
                        Y             = -887.4646f,
                        Z             = -536.03f,
                        Rx            = -0.7632219f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Chua - Level 50"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 5,
                        Faction       = 166,
                        CreationStart = 4,
                        WorldId       = 3460,
                        X             = 29.1286f,
                        Y             = -853.8716f,
                        Z             = -560.188f,
                        Rx            = -2.751458f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Draken - Novice"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 5,
                        Faction       = 166,
                        CreationStart = 3,
                        WorldId       = 870,
                        X             = -8261.3984f,
                        Y             = -995.471f,
                        Z             = -242.3648f,
                        Rx            = -2.215535f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Draken - Veteran"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 5,
                        Faction       = 166,
                        CreationStart = 5,
                        WorldId       = 22,
                        X             = -3343.58f,
                        Y             = -887.4646f,
                        Z             = -536.03f,
                        Rx            = -0.7632219f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Draken - Level 50"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 1,
                        Faction       = 166,
                        CreationStart = 4,
                        WorldId       = 3460,
                        X             = 29.1286f,
                        Y             = -853.8716f,
                        Z             = -560.188f,
                        Rx            = -2.751458f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Cassian - Novice"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 1,
                        Faction       = 166,
                        CreationStart = 3,
                        WorldId       = 1387,
                        X             = -3835.341f,
                        Y             = -980.2174f,
                        Z             = -6050.524f,
                        Rx            = -0.456820f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Cassian - Veteran"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 1,
                        Faction       = 166,
                        CreationStart = 5,
                        WorldId       = 22,
                        X             = -3343.58f,
                        Y             = -887.4646f,
                        Z             = -536.03f,
                        Rx            = -0.7632219f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Cassian - Level 50"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 12,
                        Faction       = 166,
                        CreationStart = 4,
                        WorldId       = 3460,
                        X             = 29.1286f,
                        Y             = -853.8716f,
                        Z             = -560.188f,
                        Rx            = -2.751458f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Mechari - Novice"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 12,
                        Faction       = 166,
                        CreationStart = 3,
                        WorldId       = 1387,
                        X             = -3835.341f,
                        Y             = -980.2174f,
                        Z             = -6050.524f,
                        Rx            = -0.456820f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Mechari - Veteran"
                    },
                    new CharacterCreateModel
                    {
                        Race          = 12,
                        Faction       = 166,
                        CreationStart = 5,
                        WorldId       = 22,
                        X             = -3343.58f,
                        Y             = -887.4646f,
                        Z             = -536.03f,
                        Rx            = -0.7632219f,
                        Ry            = 0f,
                        Rz            = 0f,
                        Comment       = "Dominion Mechari - Level 50"
                    });
            });

            modelBuilder.Entity<CharacterCurrencyModel>(entity =>
            {
                entity.ToTable("character_currency");

                entity.HasKey(e => new { e.Id, e.CurrencyId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CurrencyId)
                    .HasColumnName("currencyId")
                    .HasColumnType("tinyint(4) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Currency)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK_character_currency_id__character_id");
            });

            modelBuilder.Entity<CharacterCustomisationModel>(entity =>
            {
                entity.ToTable("character_customisation");

                entity.HasKey(e => new { e.Id, e.Label })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Label)
                    .HasColumnName("label")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Customisation)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_customisation_id__character_id");
            });

            modelBuilder.Entity<CharacterDatacubeModel>(entity =>
            {
                entity.ToTable("character_datacube");

                entity.HasKey(e => new { e.Id, e.Type, e.Datacube })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Datacube)
                    .HasColumnName("datacube")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Progress)
                    .HasColumnName("progress")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Datacube)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_datacube_id__character_id");
            });

            modelBuilder.Entity<CharacterEntitlementModel>(entity =>
            {
                entity.ToTable("character_entitlement");

                entity.HasKey(e => new { e.Id, e.EntitlementId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.EntitlementId)
                    .HasColumnName("entitlementId")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Entitlement)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_entitlement_id__character_id");
            });

            modelBuilder.Entity<CharacterKeybindingModel>(entity =>
            {
                entity.ToTable("character_keybinding");

                entity.HasKey(e => new { e.Id, e.InputActionId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.InputActionId)
                    .HasColumnName("inputActionId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Code00)
                    .HasColumnName("code00")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Code01)
                    .HasColumnName("code01")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Code02)
                    .HasColumnName("code02")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DeviceEnum00)
                    .HasColumnName("deviceEnum00")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DeviceEnum01)
                    .HasColumnName("deviceEnum01")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DeviceEnum02)
                    .HasColumnName("deviceEnum02")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.EventTypeEnum00)
                    .HasColumnName("eventTypeEnum00")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.EventTypeEnum01)
                    .HasColumnName("eventTypeEnum01")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.EventTypeEnum02)
                    .HasColumnName("eventTypeEnum02")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.MetaKeys00)
                    .HasColumnName("metaKeys00")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.MetaKeys01)
                    .HasColumnName("metaKeys01")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.MetaKeys02)
                    .HasColumnName("metaKeys02")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Keybinding)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_keybinding_id__character_id");
            });

            modelBuilder.Entity<CharacterMailModel>(entity =>
            {
                entity.ToTable("character_mail");

                entity.HasIndex(e => e.RecipientId)
                    .HasDatabaseName("FK__character_mail_recipientId__character_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.CreatureId)
                    .HasColumnName("creatureId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CurrencyAmount)
                    .HasColumnName("currencyAmount")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CurrencyType)
                    .HasColumnName("currencyType")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DeliveryTime)
                    .HasColumnName("deliveryTime")
                    .HasColumnType("tinyint(8) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasColumnType("tinyint(8) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.HasPaidOrCollectedCurrency)
                    .HasColumnName("hasPaidOrCollectedCurrency")
                    .HasColumnType("tinyint(8) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.IsCashOnDelivery)
                    .HasColumnName("isCashOnDelivery")
                    .HasColumnType("tinyint(8) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasColumnType("varchar(2000)")
                    .HasDefaultValue("");

                entity.Property(e => e.RecipientId)
                    .HasColumnName("recipientId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.SenderId)
                    .HasColumnName("senderId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.SenderType)
                    .HasColumnName("senderType")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnName("subject")
                    .HasColumnType("varchar(200)")
                    .HasDefaultValue("");

                entity.Property(e => e.TextEntryMessage)
                    .HasColumnName("textEntryMessage")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.TextEntrySubject)
                    .HasColumnName("textEntrySubject")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.Mail)
                    .HasForeignKey(d => d.RecipientId)
                    .HasConstraintName("FK__character_mail_recipientId__character_id");
            });

            modelBuilder.Entity<CharacterMailAttachmentModel>(entity =>
            {
                entity.ToTable("character_mail_attachment");

                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ItemGuid)
                    .HasDatabaseName("itemGuid")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.ItemGuid)
                    .HasColumnName("itemGuid")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Mail)
                    .WithMany(p => p.Attachment)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_mail_attachment_id__character_mail_id");

                entity.HasOne(d => d.Item)
                    .WithOne(p => p.MailAttachment)
                    .HasForeignKey<CharacterMailAttachmentModel>(d => d.ItemGuid)
                    .HasConstraintName("FK__character_mail_attachment_itemGuid__item_id");
            });

            modelBuilder.Entity<CharacterPathModel>(entity =>
            {
                entity.ToTable("character_path");

                entity.HasKey(e => new { e.Id, e.Path })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Path)
                    .HasColumnName("path")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.LevelRewarded)
                    .HasColumnName("levelRewarded")
                    .HasColumnType("tinyint(4) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.TotalXp)
                    .HasColumnName("totalXp")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Unlocked)
                    .HasColumnName("unlocked")
                    .HasColumnType("tinyint(1) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Path)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_path_id__character_id");
            });

            modelBuilder.Entity<CharacterPetCustomisationModel>(entity =>
            {
                entity.ToTable("character_pet_customisation");

                entity.HasKey(e => new { e.Id, e.Type, e.ObjectId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ObjectId)
                    .HasColumnName("objectId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.FlairIdMask)
                    .HasColumnName("flairIdMask")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValue("");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.PetCustomisation)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_pet_customisation_id__character_id");
            });

            modelBuilder.Entity<CharacterPetFlairModel>(entity =>
            {
                entity.ToTable("character_pet_flair");

                entity.HasKey(e => new { e.Id, e.PetFlairId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.PetFlairId)
                    .HasColumnName("petFlairId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.PetFlair)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_pet_flair_id__character_id");
            });

            modelBuilder.Entity<CharacterQuestModel>(entity =>
            {
                entity.ToTable("character_quest");

                entity.HasKey(e => new { e.Id, e.QuestId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.QuestId)
                    .HasColumnName("questId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Reset)
                    .HasColumnName("reset")
                    .HasColumnType("datetime")
                    .HasDefaultValue(null);

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Timer)
                    .HasColumnName("timer")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(null);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Quest)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_quest_id__character_id");
            });

            modelBuilder.Entity<CharacterQuestObjectiveModel>(entity =>
            {
                entity.ToTable("character_quest_objective");

                entity.HasKey(e => new { e.Id, e.QuestId, e.Index })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.QuestId)
                    .HasColumnName("questId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Progress)
                    .HasColumnName("progress")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Timer)
                    .HasColumnName("timer")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(null);

                entity.HasOne(d => d.Quest)
                    .WithMany(p => p.QuestObjective)
                    .HasForeignKey(d => new { d.Id, d.QuestId })
                    .HasConstraintName("FK__character_quest_objective_id__character_id");
            });

            modelBuilder.Entity<CharacterReputation>(entity =>
            {
                entity.ToTable("character_reputation");

                entity.HasKey(e => new { e.Id, e.FactionId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.FactionId)
                    .HasColumnName("factionId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Reputation)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_reputation_id__character_id");
            });

            modelBuilder.Entity<CharacterSpellModel>(entity =>
            {
                entity.ToTable("character_spell");

                entity.HasKey(e => new { e.Id, e.Spell4BaseId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Spell4BaseId)
                    .HasColumnName("spell4BaseId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Tier)
                    .HasColumnName("tier")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Spell)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_spell_id__character_id");
            });

            modelBuilder.Entity<CharacterStatModel>(entity =>
            {
                entity.ToTable("character_stats");

                entity.HasKey(e => new { e.Id, e.Stat })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Stat)
                    .HasColumnName("stat")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Stat)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_stats_stat_id_character_id");
            });

            modelBuilder.Entity<CharacterTitleModel>(entity =>
            {
                entity.ToTable("character_title");

                entity.HasKey(e => new { e.Id, e.Title })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Revoked)
                    .HasColumnName("revoked")
                    .HasColumnType("tinyint(4) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.TimeRemaining)
                    .HasColumnName("timeRemaining")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharacterTitle)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_title_id__character_id");
            });

            modelBuilder.Entity<CharacterTradeskillMaterialModel>(entity =>
            {
                entity.ToTable("character_tradeskill_materials");

                entity.HasKey(e => new { e.Id, e.MaterialId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.MaterialId)
                    .HasColumnName("materialId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.TradeskillMaterials)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_tradeskill_material_id__character_id");
            });

            modelBuilder.Entity<CharacterZonemapHexgroupModel>(entity =>
            {
                entity.ToTable("character_zonemap_hexgroup");

                entity.HasKey(e => new { e.Id, e.ZoneMap, e.HexGroup })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ZoneMap)
                    .HasColumnName("zoneMap")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.HexGroup)
                    .HasColumnName("hexGroup")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.ZonemapHexgroup)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_zonemap_hexgroup_id__character_id");
            });

            modelBuilder.Entity<ChatChannelModel>(entity =>
            {
                entity.ToTable("chat_channel");

                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(20)")
                    .HasDefaultValue("");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasColumnType("varchar(20)")
                    .HasDefaultValue("");
            });

            modelBuilder.Entity<ChatChannelMemberModel>(entity =>
            {
                entity.ToTable("chat_channel_member");

                entity.HasKey(e => new { e.Id, e.CharacterId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CharacterId)
                    .HasColumnName("characterId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(e => e.Channel)
                    .WithMany(e => e.Members)
                    .HasForeignKey(e => e.Id)
                    .HasConstraintName("FK__chat_channel_member_id__chat_channel_id");
            });

            modelBuilder.Entity<GuildModel>(entity =>
            {
                entity.HasKey(e => new { e.Id })
                    .HasName("PRIMARY");

                entity.ToTable("guild");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(30)")
                    .HasDefaultValue("");

                entity.Property(e => e.LeaderId)
                    .HasColumnName("leaderId")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.DeleteTime)
                    .HasColumnName("deleteTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.OriginalName)
                    .HasColumnName("originalName")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.OriginalLeaderId)
                    .HasColumnName("originalLeaderId")
                    .HasColumnType("bigint(20) unsigned");
            });

            modelBuilder.Entity<GuildAchievementModel>(entity =>
            {
                entity.ToTable("guild_achievement");

                entity.HasKey(e => new { e.Id, e.AchievementId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.AchievementId)
                    .HasColumnName("achievementId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Data0)
                    .HasColumnName("data0")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Data1)
                    .HasColumnName("data1")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DateCompleted)
                    .HasColumnName("dateCompleted")
                    .HasColumnType("datetime")
                    .HasDefaultValue(null);

                entity.HasOne(d => d.Guild)
                    .WithMany(p => p.Achievement)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__guild_achievement_id__guild_id");
            });

            modelBuilder.Entity<GuildDataModel>(entity =>
            {
                entity.HasKey(e => new { e.Id })
                    .HasName("PRIMARY");

                entity.ToTable("guild_guild_data");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.MessageOfTheDay)
                    .HasColumnName("motd")
                    .HasColumnType("varchar(200)")
                    .HasDefaultValue("");

                entity.Property(e => e.AdditionalInfo)
                    .HasColumnName("additionalInfo")
                    .HasColumnType("varchar(400)")
                    .HasDefaultValue("");

                entity.Property(e => e.BackgroundIconPartId)
                    .HasColumnName("backgroundIconPartId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ForegroundIconPartId)
                    .HasColumnName("foregroundIconPartId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ScanLinesPartId)
                    .HasColumnName("scanLinesPartId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Guild)
                    .WithOne(p => p.GuildData)
                    .HasForeignKey<GuildDataModel>(d => d.Id)
                    .HasConstraintName("FK__guild_guild_data_id__guild_id");
            });

            modelBuilder.Entity<GuildRankModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("guild_rank");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(16)")
                    .HasDefaultValue("");

                entity.Property(e => e.Permission)
                    .HasColumnName("permission")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.BankWithdrawalPermission)
                    .HasColumnName("bankWithdrawPermission")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.MoneyWithdrawalLimit)
                    .HasColumnName("moneyWithdrawalLimit")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.RepairLimit)
                    .HasColumnName("repairLimit")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Guild)
                    .WithMany(p => p.GuildRank)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__guild_rank_id__guild_id");
            });

            modelBuilder.Entity<GuildMemberModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CharacterId })
                    .HasName("PRIMARY");

                entity.ToTable("guild_member");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CharacterId)
                    .HasColumnName("characterId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Rank)
                    .HasColumnName("rank")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValue("");

                entity.Property(e => e.CommunityPlotReservation)
                    .HasColumnName("communityPlotReservation")
                    .HasColumnType("int(11)")
                    .HasDefaultValue(-1);

                entity.HasOne(d => d.Guild)
                    .WithMany(p => p.GuildMember)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__guild_member_id__guild_id");
            });

            modelBuilder.Entity<ItemModel>(entity =>
            {
                entity.ToTable("item");

                entity.HasIndex(e => e.OwnerId)
                    .HasDatabaseName("FK__item_ownerId__character_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.BagIndex)
                    .HasColumnName("bagIndex")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Charges)
                    .HasColumnName("charges")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Durability)
                    .HasColumnName("durability")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.ExpirationTimeLeft)
                    .HasColumnName("expirationTimeLeft")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.OwnerId)
                    .HasColumnName("ownerId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(null);

                entity.Property(e => e.StackCount)
                    .HasColumnName("stackCount")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__item_ownerId__character_id");
            });

            modelBuilder.Entity<PropertyBaseModel>(entity =>
            {
                entity.HasKey(e => new { e.Type, e.Subtype, e.Property, })
                    .HasName("PRIMARY");

                entity.ToTable("property_base");

                entity.Property(e => e.Type)
                    .ValueGeneratedNever()
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Property)
                    .ValueGeneratedNever()
                    .HasColumnName("property")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Subtype)
                    .ValueGeneratedNever()
                    .HasColumnName("subtype")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModType)
                    .HasColumnName("modType")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnName("note")
                    .HasColumnType("varchar(100)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasDefaultValueSql("'0'");

                entity.HasData(
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 0,
                        ModType = 0,
                        Value = 0,
                        Note = "Player - Base Strength"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 1,
                        ModType = 0,
                        Value = 0,
                        Note = "Player - Base Dexterity"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 2,
                        ModType = 0,
                        Value = 0,
                        Note = "Player - Base Technology"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 3,
                        ModType = 0,
                        Value = 0,
                        Note = "Player - Base Magic"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 4,
                        ModType = 0,
                        Value = 0,
                        Note = "Player - Base Wisdom"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 7,
                        ModType = 3,
                        Value = 200,
                        Note = "Player - Base HP per Level"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 9,
                        ModType = 0,
                        Value = 500,
                        Note = "Player - Base Endurance"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 16,
                        ModType = 0,
                        Value = 0.0225f,
                        Note = "Player - Base Endurance Regen"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 35,
                        ModType = 3,
                        Value = 18,
                        Note = "Player - Base Assault Rating per Level"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 36,
                        ModType = 3,
                        Value = 18,
                        Note = "Player - Base Support Rating per Level"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 38,
                        ModType = 0,
                        Value = 200,
                        Note = "Player - Base Dash Energy"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 39,
                        ModType = 0,
                        Value = 0.045f,
                        Note = "Player - Base Dash Energy Regen"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 41,
                        ModType = 0,
                        Value = 0,
                        Note = "Player - Shield Capacity Base"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 100,
                        ModType = 0,
                        Value = 1,
                        Note = "Player - Base Movement Speed"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 101,
                        ModType = 0,
                        Value = 0.05f,
                        Note = "Player - Base Avoid Chance"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 102,
                        ModType = 0,
                        Value = 0.05f,
                        Note = "Player - Base Crit Chance"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 107,
                        ModType = 0,
                        Value = 0,
                        Note = "Player - Base Focus Recovery In Combat"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 108,
                        ModType = 0,
                        Value = 0,
                        Note = "Player - Base Focus Recovery Out of Combat"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 112,
                        ModType = 0,
                        Value = 0.3f,
                        Note = "Player - Base Multi-Hit Amount"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 130,
                        ModType = 0,
                        Value = 0.8f,
                        Note = "Player - Base Gravity Multiplier"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 150,
                        ModType = 0,
                        Value = 1,
                        Note = "Player - Base Damage Taken Offset - Physical"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 151,
                        ModType = 0,
                        Value = 1,
                        Note = "Player - Base Damage Taken Offset - Tech"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 152,
                        ModType = 0,
                        Value = 1,
                        Note = "Player - Base Damage Taken Offset - Magic"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 154,
                        ModType = 0,
                        Value = 0.05f,
                        Note = "Player - Base Mutli-Hit Chance"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 155,
                        ModType = 0,
                        Value = 0.05f,
                        Note = "Player - Base Damage Reflect Amount"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 191,
                        ModType = 0,
                        Value = 1,
                        Note = "Player - Base Mount Movement Speed"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 195,
                        ModType = 0,
                        Value = 0.3f,
                        Note = "Player - Base Glance Amount"
                    },
                    new PropertyBaseModel
                    {
                        Type = 1,
                        Subtype = 1,
                        Property = 10,
                        ModType = 0,
                        Value = 1000,
                        Note = "Class - Warrior - Base Kinetic Energy Cap"
                    },
                    new PropertyBaseModel
                    {
                        Type = 0,
                        Subtype = 0,
                        Property = 17,
                        ModType = 0,
                        Value = 1,
                        Note = "Warrior - Base Kinetic Energy Regen"
                    },
                    new PropertyBaseModel
                    {
                        Type = 1,
                        Subtype = 2,
                        Property = 10,
                        ModType = 0,
                        Value = 100,
                        Note = "Class - Engineer - Base Volatile Energy Cap"
                    },
                    new PropertyBaseModel
                    {
                        Type = 1,
                        Subtype = 3,
                        Property = 10,
                        ModType = 0,
                        Value = 5,
                        Note = "Class - Esper - Base Psi Point Cap"
                    },
                    new PropertyBaseModel
                    {
                        Type = 1,
                        Subtype = 4,
                        Property = 10,
                        ModType = 0,
                        Value = 4,
                        Note = "Class - Medic - Base Medic Core Cap"
                    },
                    new PropertyBaseModel
                    {
                        Type = 1,
                        Subtype = 5,
                        Property = 12,
                        ModType = 0,
                        Value = 100,
                        Note = "Class - Stalker - Base Suit Power Cap"
                    },
                    new PropertyBaseModel
                    {
                        Type = 1,
                        Subtype = 5,
                        Property = 19,
                        ModType = 0,
                        Value = 0.035f,
                        Note = "Class - Stalker - Base Suit Power Regeneration Rate"
                    },
                    new PropertyBaseModel
                    {
                        Type = 1,
                        Subtype = 7,
                        Property = 13,
                        ModType = 0,
                        Value = 100,
                        Note = "Class - Spellslinger - Base Spell Power Cap"
                    });
            });

            modelBuilder.Entity<ResidenceModel>(entity =>
            {
                entity.ToTable("residence");

                entity.HasIndex(e => e.OwnerId)
                    .HasDatabaseName("ownerId")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DoorDecorInfoId)
                    .HasColumnName("doorDecorInfoId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.EntrywayDecorInfoId)
                    .HasColumnName("entrywayDecorInfoId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.GardenSharing)
                    .HasColumnName("gardenSharing")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.GroundWallpaperId)
                    .HasColumnName("groundWallpaperId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.MusicId)
                    .HasColumnName("musicId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValue("");

                entity.Property(e => e.OwnerId)
                    .HasColumnName("ownerId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(null);

                entity.Property(e => e.GuildOwnerId)
                    .HasColumnName("guildOwnerId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(null);

                entity.Property(e => e.PrivacyLevel)
                    .HasColumnName("privacyLevel")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.PropertyInfoId)
                    .HasColumnName("propertyInfoId")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ResourceSharing)
                    .HasColumnName("resourceSharing")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.RoofDecorInfoId)
                    .HasColumnName("roofDecorInfoId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.SkyWallpaperId)
                    .HasColumnName("skyWallpaperId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.WallpaperId)
                    .HasColumnName("wallpaperId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Character)
                    .WithOne(p => p.Residence)
                    .HasForeignKey<ResidenceModel>(d => d.OwnerId)
                    .HasConstraintName("FK__residence_ownerId__character_id");

                entity.HasOne(d => d.Guild)
                    .WithMany(p => p.Residence)
                    .HasForeignKey(d => d.GuildOwnerId)
                    .HasConstraintName("FK__residence_guildOwnerId__guild_id");
            });

            modelBuilder.Entity<ResidenceDecor>(entity =>
            {
                entity.ToTable("residence_decor");

                entity.HasKey(e => new { e.Id, e.DecorId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DecorId)
                    .HasColumnName("decorId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ColourShiftId)
                    .HasColumnName("colourShiftId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DecorInfoId)
                    .HasColumnName("decorInfoId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DecorParentId)
                    .HasColumnName("decorParentId")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.DecorType)
                    .HasColumnName("decorType")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.PlotIndex)
                    .HasColumnName("plotIndex")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(2147483647)
                    .ValueGeneratedNever();

                entity.Property(e => e.Qw)
                    .HasColumnName("qw")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Qx)
                    .HasColumnName("qx")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Qy)
                    .HasColumnName("qy")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Qz)
                    .HasColumnName("qz")
                    .HasColumnType("float")
                    .HasDefaultValue(0);

                entity.Property(e => e.Scale)
                    .HasColumnName("scale")
                    .HasColumnType("float")
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

                entity.HasOne(d => d.Residence)
                    .WithMany(p => p.Decor)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__residence_decor_id__residence_id");
            });

            modelBuilder.Entity<ResidencePlotModel>(entity =>
            {
                entity.ToTable("residence_plot");

                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.BuildState)
                    .HasColumnName("buildState")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.PlotInfoId)
                    .HasColumnName("plotInfoId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.PlugFacing)
                    .HasColumnName("plugFacing")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.PlugItemId)
                    .HasColumnName("plugItemId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Residence)
                    .WithMany(p => p.Plot)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__residence_plot_id__residence_id");
            });
        }
    }
}
