using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;

namespace NexusForever.WorldServer.Database.Character.Model
{
    public partial class CharacterContext : DbContext
    {
        public CharacterContext()
        {
        }

        public CharacterContext(DbContextOptions<CharacterContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Character> Character { get; set; }
        public virtual DbSet<CharacterAchievement> CharacterAchievement { get; set; }
        public virtual DbSet<CharacterActionSetAmp> CharacterActionSetAmp { get; set; }
        public virtual DbSet<CharacterActionSetShortcut> CharacterActionSetShortcut { get; set; }
        public virtual DbSet<CharacterAppearance> CharacterAppearance { get; set; }
        public virtual DbSet<CharacterBone> CharacterBone { get; set; }
        public virtual DbSet<CharacterCostume> CharacterCostume { get; set; }
        public virtual DbSet<CharacterCostumeItem> CharacterCostumeItem { get; set; }
        public virtual DbSet<CharacterCurrency> CharacterCurrency { get; set; }
        public virtual DbSet<CharacterCustomisation> CharacterCustomisation { get; set; }
        public virtual DbSet<CharacterDatacube> CharacterDatacube { get; set; }
        public virtual DbSet<CharacterEntitlement> CharacterEntitlement { get; set; }
        public virtual DbSet<CharacterKeybinding> CharacterKeybinding { get; set; }
        public virtual DbSet<CharacterMail> CharacterMail { get; set; }
        public virtual DbSet<CharacterMailAttachment> CharacterMailAttachment { get; set; }
        public virtual DbSet<CharacterPath> CharacterPath { get; set; }
        public virtual DbSet<CharacterPetCustomisation> CharacterPetCustomisation { get; set; }
        public virtual DbSet<CharacterPetFlair> CharacterPetFlair { get; set; }
        public virtual DbSet<CharacterQuest> CharacterQuest { get; set; }
        public virtual DbSet<CharacterQuestObjective> CharacterQuestObjective { get; set; }
        public virtual DbSet<CharacterSpell> CharacterSpell { get; set; }
        public virtual DbSet<CharacterStats> CharacterStats { get; set; }
        public virtual DbSet<CharacterTitle> CharacterTitle { get; set; }
        public virtual DbSet<CharacterZonemapHexgroup> CharacterZonemapHexgroup { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<Residence> Residence { get; set; }
        public virtual DbSet<ResidenceDecor> ResidenceDecor { get; set; }
        public virtual DbSet<ResidencePlot> ResidencePlot { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(DatabaseManager.Config, DatabaseType.Character);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Character>(entity =>
            {
                entity.ToTable("character");

                entity.HasIndex(e => e.AccountId)
                    .HasName("accountId");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.AccountId)
                    .HasColumnName("accountId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ActiveCostumeIndex)
                    .HasColumnName("activeCostumeIndex")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'-1'");

                entity.Property(e => e.ActivePath)
                    .HasColumnName("activePath")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ActiveSpec)
                    .HasColumnName("activeSpec")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Class)
                    .HasColumnName("class")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.DeleteTime)
                    .HasColumnName("deleteTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.FactionId)
                    .HasColumnName("factionId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.InnateIndex)
                    .HasColumnName("innateIndex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.InputKeySet)
                    .HasColumnName("inputKeySet")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LastOnline)
                    .HasColumnName("lastOnline")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LocationX)
                    .HasColumnName("locationX")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LocationY)
                    .HasColumnName("locationY")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LocationZ)
                    .HasColumnName("locationZ")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.OriginalName)
                    .HasColumnName("originalName")
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.PathActivatedTimestamp)
                    .HasColumnName("pathActivatedTimestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Race)
                    .HasColumnName("race")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Sex)
                    .HasColumnName("sex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TimePlayedLevel)
                    .HasColumnName("timePlayedLevel")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TimePlayedTotal)
                    .HasColumnName("timePlayedTotal")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.WorldId)
                    .HasColumnName("worldId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.WorldZoneId)
                    .HasColumnName("worldZoneId")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<CharacterAchievement>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.AchievementId })
                    .HasName("PRIMARY");

                entity.ToTable("character_achievement");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.AchievementId)
                    .HasColumnName("achievementId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Data0)
                    .HasColumnName("data0")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Data1)
                    .HasColumnName("data1")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DateCompleted)
                    .HasColumnName("dateCompleted")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterAchievement)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_achievement_id__character_id");
            });

            modelBuilder.Entity<CharacterActionSetAmp>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.SpecIndex, e.AmpId })
                    .HasName("PRIMARY");

                entity.ToTable("character_action_set_amp");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SpecIndex)
                    .HasColumnName("specIndex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.AmpId)
                    .HasColumnName("ampId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterActionSetAmp)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_action_set_amp_id__character_id");
            });

            modelBuilder.Entity<CharacterActionSetShortcut>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.SpecIndex, e.Location })
                    .HasName("PRIMARY");

                entity.ToTable("character_action_set_shortcut");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SpecIndex)
                    .HasColumnName("specIndex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ObjectId)
                    .HasColumnName("objectId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ShortcutType)
                    .HasColumnName("shortcutType")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tier)
                    .HasColumnName("tier")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterActionSetShortcut)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_action_set_shortcut_id__character_id");
            });

            modelBuilder.Entity<CharacterAppearance>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Slot })
                    .HasName("PRIMARY");

                entity.ToTable("character_appearance");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Slot)
                    .HasColumnName("slot")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DisplayId)
                    .HasColumnName("displayId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterAppearance)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_appearance_id__character_id");
            });

            modelBuilder.Entity<CharacterBone>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.BoneIndex })
                    .HasName("PRIMARY");

                entity.ToTable("character_bone");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BoneIndex)
                    .HasColumnName("boneIndex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Bone)
                    .HasColumnName("bone")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterBone)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK_character_bone_id__character_id");
            });

            modelBuilder.Entity<CharacterCostume>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("character_costume");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Mask)
                    .HasColumnName("mask")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterCostume)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_costume_id__character_id");
            });

            modelBuilder.Entity<CharacterCostumeItem>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index, e.Slot })
                    .HasName("PRIMARY");

                entity.ToTable("character_costume_item");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Slot)
                    .HasColumnName("slot")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DyeData)
                    .HasColumnName("dyeData")
                    .HasColumnType("int(10)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.I)
                    .WithMany(p => p.CharacterCostumeItem)
                    .HasForeignKey(d => new { d.Id, d.Index })
                    .HasConstraintName("FK__character_costume_item_id-index__character_costume_id-index");
            });

            modelBuilder.Entity<CharacterCurrency>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CurrencyId })
                    .HasName("PRIMARY");

                entity.ToTable("character_currency");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CurrencyId)
                    .HasColumnName("currencyId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterCurrency)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK_character_currency_id__character_id");
            });

            modelBuilder.Entity<CharacterCustomisation>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Label })
                    .HasName("PRIMARY");

                entity.ToTable("character_customisation");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Label)
                    .HasColumnName("label")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterCustomisation)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_customisation_id__character_id");
            });

            modelBuilder.Entity<CharacterDatacube>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.Datacube })
                    .HasName("PRIMARY");

                entity.ToTable("character_datacube");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Datacube)
                    .HasColumnName("datacube")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Progress)
                    .HasColumnName("progress")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterDatacube)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_datacube_id__character_id");
            });

            modelBuilder.Entity<CharacterEntitlement>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.EntitlementId })
                    .HasName("PRIMARY");

                entity.ToTable("character_entitlement");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.EntitlementId)
                    .HasColumnName("entitlementId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterEntitlement)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_entitlement_id__character_id");
            });

            modelBuilder.Entity<CharacterKeybinding>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.InputActionId })
                    .HasName("PRIMARY");

                entity.ToTable("character_keybinding");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.InputActionId)
                    .HasColumnName("inputActionId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Code00)
                    .HasColumnName("code00")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Code01)
                    .HasColumnName("code01")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Code02)
                    .HasColumnName("code02")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DeviceEnum00)
                    .HasColumnName("deviceEnum00")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DeviceEnum01)
                    .HasColumnName("deviceEnum01")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DeviceEnum02)
                    .HasColumnName("deviceEnum02")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.EventTypeEnum00)
                    .HasColumnName("eventTypeEnum00")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.EventTypeEnum01)
                    .HasColumnName("eventTypeEnum01")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.EventTypeEnum02)
                    .HasColumnName("eventTypeEnum02")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MetaKeys00)
                    .HasColumnName("metaKeys00")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MetaKeys01)
                    .HasColumnName("metaKeys01")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MetaKeys02)
                    .HasColumnName("metaKeys02")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterKeybinding)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_keybinding_id__character_id");
            });

            modelBuilder.Entity<CharacterMail>(entity =>
            {
                entity.ToTable("character_mail");

                entity.HasIndex(e => e.RecipientId)
                    .HasName("FK__character_mail_recipientId__character_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.CreatureId)
                    .HasColumnName("creatureId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CurrencyAmount)
                    .HasColumnName("currencyAmount")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CurrencyType)
                    .HasColumnName("currencyType")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DeliveryTime)
                    .HasColumnName("deliveryTime")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.HasPaidOrCollectedCurrency)
                    .HasColumnName("hasPaidOrCollectedCurrency")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IsCashOnDelivery)
                    .HasColumnName("isCashOnDelivery")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasColumnType("varchar(2000)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.RecipientId)
                    .HasColumnName("recipientId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SenderId)
                    .HasColumnName("senderId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SenderType)
                    .HasColumnName("senderType")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnName("subject")
                    .HasColumnType("varchar(200)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.TextEntryMessage)
                    .HasColumnName("textEntryMessage")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TextEntrySubject)
                    .HasColumnName("textEntrySubject")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.CharacterMail)
                    .HasForeignKey(d => d.RecipientId)
                    .HasConstraintName("FK__character_mail_recipientId__character_id");
            });

            modelBuilder.Entity<CharacterMailAttachment>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("character_mail_attachment");

                entity.HasIndex(e => e.ItemGuid)
                    .HasName("itemGuid")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemGuid)
                    .HasColumnName("itemGuid")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterMailAttachment)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_mail_attachment_id__character_mail_id");

                entity.HasOne(d => d.ItemGu)
                    .WithOne(p => p.CharacterMailAttachment)
                    .HasForeignKey<CharacterMailAttachment>(d => d.ItemGuid)
                    .HasConstraintName("FK__character_mail_attachment_itemGuid__item_id");
            });

            modelBuilder.Entity<CharacterPath>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Path })
                    .HasName("PRIMARY");

                entity.ToTable("character_path");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Path)
                    .HasColumnName("path")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LevelRewarded)
                    .HasColumnName("levelRewarded")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TotalXp)
                    .HasColumnName("totalXp")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Unlocked)
                    .HasColumnName("unlocked")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterPath)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_path_id__character_id");
            });

            modelBuilder.Entity<CharacterPetCustomisation>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Type, e.ObjectId })
                    .HasName("PRIMARY");

                entity.ToTable("character_pet_customisation");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ObjectId)
                    .HasColumnName("objectId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.FlairIdMask)
                    .HasColumnName("flairIdMask")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterPetCustomisation)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_pet_customisation_id__character_id");
            });

            modelBuilder.Entity<CharacterPetFlair>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.PetFlairId })
                    .HasName("PRIMARY");

                entity.ToTable("character_pet_flair");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.PetFlairId)
                    .HasColumnName("petFlairId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterPetFlair)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_pet_flair_id__character_id");
            });

            modelBuilder.Entity<CharacterQuest>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.QuestId })
                    .HasName("PRIMARY");

                entity.ToTable("character_quest");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.QuestId)
                    .HasColumnName("questId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Reset)
                    .HasColumnName("reset")
                    .HasColumnType("datetime");

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timer).HasColumnName("timer");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterQuest)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_quest_id__character_id");
            });

            modelBuilder.Entity<CharacterQuestObjective>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.QuestId, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("character_quest_objective");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.QuestId)
                    .HasColumnName("questId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Progress)
                    .HasColumnName("progress")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timer).HasColumnName("timer");

                entity.HasOne(d => d.CharacterQuest)
                    .WithMany(p => p.CharacterQuestObjective)
                    .HasForeignKey(d => new { d.Id, d.QuestId })
                    .HasConstraintName("FK__character_quest_objective_id__character_id");
            });

            modelBuilder.Entity<CharacterSpell>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Spell4BaseId })
                    .HasName("PRIMARY");

                entity.ToTable("character_spell");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Spell4BaseId)
                    .HasColumnName("spell4BaseId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Tier)
                    .HasColumnName("tier")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterSpell)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_spell_id__character_id");
            });

            modelBuilder.Entity<CharacterStats>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Stat })
                    .HasName("PRIMARY");

                entity.ToTable("character_stats");

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
                    .WithMany(p => p.CharacterStats)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_stats_stat_id_character_id");
            });

            modelBuilder.Entity<CharacterTitle>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Title })
                    .HasName("PRIMARY");

                entity.ToTable("character_title");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Revoked)
                    .HasColumnName("revoked")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.TimeRemaining)
                    .HasColumnName("timeRemaining")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterTitle)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_title_id__character_id");
            });

            modelBuilder.Entity<CharacterZonemapHexgroup>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.ZoneMap, e.HexGroup })
                    .HasName("PRIMARY");

                entity.ToTable("character_zonemap_hexgroup");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ZoneMap)
                    .HasColumnName("zoneMap")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.HexGroup)
                    .HasColumnName("hexGroup")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.CharacterZonemapHexgroup)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__character_zonemap_hexgroup_id__character_id");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("item");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("FK__item_ownerId__character_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BagIndex)
                    .HasColumnName("bagIndex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Charges)
                    .HasColumnName("charges")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Durability)
                    .HasColumnName("durability")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ExpirationTimeLeft)
                    .HasColumnName("expirationTimeLeft")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Location)
                    .HasColumnName("location")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.OwnerId).HasColumnName("ownerId");

                entity.Property(e => e.StackCount)
                    .HasColumnName("stackCount")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__item_ownerId__character_id");
            });

            modelBuilder.Entity<Residence>(entity =>
            {
                entity.ToTable("residence");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("ownerId")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DoorDecorInfoId)
                    .HasColumnName("doorDecorInfoId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.EntrywayDecorInfoId)
                    .HasColumnName("entrywayDecorInfoId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.GardenSharing)
                    .HasColumnName("gardenSharing")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.GroundWallpaperId)
                    .HasColumnName("groundWallpaperId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MusicId)
                    .HasColumnName("musicId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.OwnerId)
                    .HasColumnName("ownerId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.PrivacyLevel)
                    .HasColumnName("privacyLevel")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.PropertyInfoId)
                    .HasColumnName("propertyInfoId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ResourceSharing)
                    .HasColumnName("resourceSharing")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.RoofDecorInfoId)
                    .HasColumnName("roofDecorInfoId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.SkyWallpaperId)
                    .HasColumnName("skyWallpaperId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.WallpaperId)
                    .HasColumnName("wallpaperId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Owner)
                    .WithOne(p => p.Residence)
                    .HasForeignKey<Residence>(d => d.OwnerId)
                    .HasConstraintName("FK__residence_ownerId__character_id");
            });

            modelBuilder.Entity<ResidenceDecor>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.DecorId })
                    .HasName("PRIMARY");

                entity.ToTable("residence_decor");

                entity.HasIndex(e => e.DecorId)
                    .HasName("FK_residence_decor_item");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DecorId)
                    .HasColumnName("decorId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ColourShiftId)
                    .HasColumnName("colourShiftId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DecorInfoId)
                    .HasColumnName("decorInfoId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DecorParentId)
                    .HasColumnName("decorParentId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DecorType)
                    .HasColumnName("decorType")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.PlotIndex)
                    .HasColumnName("plotIndex")
                    .HasDefaultValueSql("'2147483647'");

                entity.Property(e => e.Qw)
                    .HasColumnName("qw")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Qx)
                    .HasColumnName("qx")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Qy)
                    .HasColumnName("qy")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Qz)
                    .HasColumnName("qz")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Scale)
                    .HasColumnName("scale")
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

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.ResidenceDecor)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__residence_decor_id__residence_id");
            });

            modelBuilder.Entity<ResidencePlot>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index })
                    .HasName("PRIMARY");

                entity.ToTable("residence_plot");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BuildState)
                    .HasColumnName("buildState")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.PlotInfoId)
                    .HasColumnName("plotInfoId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.PlugFacing)
                    .HasColumnName("plugFacing")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.PlugItemId)
                    .HasColumnName("plugItemId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.ResidencePlot)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__residence_plot_id__residence_id");
            });
        }
    }
}
