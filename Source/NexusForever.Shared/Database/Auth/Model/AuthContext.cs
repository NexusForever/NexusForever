using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NexusForever.Shared.Configuration;

namespace NexusForever.Shared.Database.Auth.Model
{
    public partial class AuthContext : DbContext
    {
        public AuthContext()
        {
        }

        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<AccountCostumeUnlock> AccountCostumeUnlock { get; set; }
        public virtual DbSet<AccountCurrency> AccountCurrency { get; set; }
        public virtual DbSet<AccountGenericUnlock> AccountGenericUnlock { get; set; }
        public virtual DbSet<AccountKeybinding> AccountKeybinding { get; set; }
        public virtual DbSet<Server> Server { get; set; }
        public virtual DbSet<ServerMessage> ServerMessage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(DatabaseManager.Config, DatabaseType.Auth);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.HasIndex(e => e.Email)
                    .HasName("email");

                entity.HasIndex(e => e.GameToken)
                    .HasName("gameToken");

                entity.HasIndex(e => e.SessionKey)
                    .HasName("sessionKey");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.GameToken)
                    .IsRequired()
                    .HasColumnName("gameToken")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.S)
                    .IsRequired()
                    .HasColumnName("s")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.SessionKey)
                    .IsRequired()
                    .HasColumnName("sessionKey")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.V)
                    .IsRequired()
                    .HasColumnName("v")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValueSql("''");
            });

            modelBuilder.Entity<AccountCostumeUnlock>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.ItemId })
                    .HasName("PRIMARY");

                entity.ToTable("account_costume_unlock");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.AccountCostumeUnlock)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_costume_item_id__account_id");
            });

            modelBuilder.Entity<AccountCurrency>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CurrencyId })
                    .HasName("PRIMARY");

                entity.ToTable("account_currency");

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
                    .WithMany(p => p.AccountCurrency)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_currency_id__account_id");
            });

            modelBuilder.Entity<AccountGenericUnlock>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Entry })
                    .HasName("PRIMARY");

                entity.ToTable("account_generic_unlock");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Entry)
                    .HasColumnName("entry")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.AccountGenericUnlock)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_generic_unlock_id__account_id");
            });

            modelBuilder.Entity<AccountKeybinding>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.InputActionId })
                    .HasName("PRIMARY");

                entity.ToTable("account_keybinding");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.InputActionId)
                    .HasColumnName("inputActionId")
                    .HasDefaultValueSql("'0'")
                    .ValueGeneratedNever();

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
                    .WithMany(p => p.AccountKeybinding)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_keybinding_id__account_id");
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("server");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasColumnName("host")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("'127.0.0.1'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValueSql("'NexusForever'");

                entity.Property(e => e.Port)
                    .HasColumnName("port")
                    .HasDefaultValueSql("'24000'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<ServerMessage>(entity =>
            {
                entity.HasKey(e => new { e.Index, e.Language })
                    .HasName("PRIMARY");

                entity.ToTable("server_message");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Language)
                    .HasColumnName("language")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValueSql("''");
            });
        }
    }
}
