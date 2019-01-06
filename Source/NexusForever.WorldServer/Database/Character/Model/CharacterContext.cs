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
        public virtual DbSet<CharacterAppearance> CharacterAppearance { get; set; }
        public virtual DbSet<CharacterBone> CharacterBone { get; set; }
        public virtual DbSet<CharacterCurrency> CharacterCurrency { get; set; }
        public virtual DbSet<CharacterCustomisation> CharacterCustomisation { get; set; }
        public virtual DbSet<CharacterPath> CharacterPath { get; set; }
        public virtual DbSet<CharacterTitle> CharacterTitle { get; set; }
        public virtual DbSet<Item> Item { get; set; }

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

                entity.Property(e => e.ActivePath)
                    .HasColumnName("activePath")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Class)
                    .HasColumnName("class")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.FactionId)
                    .HasColumnName("factionId")
                    .HasDefaultValueSql("'0'");

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
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

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

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.WorldId)
                    .HasColumnName("worldId")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<CharacterAppearance>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Slot });

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
                entity.HasKey(e => new { e.Id, e.BoneIndex });

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

            modelBuilder.Entity<CharacterCurrency>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CurrencyId });

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
                entity.HasKey(e => new { e.Id, e.Label });

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

            modelBuilder.Entity<CharacterPath>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Path });

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

            modelBuilder.Entity<CharacterTitle>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Title });

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

                entity.Property(e => e.OwnerId)
                    .HasColumnName("ownerId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.StackCount)
                    .HasColumnName("stackCount")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK__item_ownerId__character_id");
            });
        }
    }
}
