using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Auth.Model;
using NexusForever.Database.Configuration;

namespace NexusForever.Database.Auth
{
    public class AuthContext : DbContext
    {
        public DbSet<AccountModel> Account { get; set; }
        public DbSet<AccountCostumeUnlockModel> AccountCostumeUnlock { get; set; }
        public DbSet<AccountCurrencyModel> AccountCurrency { get; set; }
        public DbSet<AccountEntitlementModel> AccountEntitlement { get; set; }
        public DbSet<AccountGenericUnlockModel> AccountGenericUnlock { get; set; }
        public DbSet<AccountKeybindingModel> AccountKeybinding { get; set; }
        public DbSet<ServerModel> Server { get; set; }
        public DbSet<ServerMessageModel> ServerMessage { get; set; }

        private readonly IDatabaseConfig config;

        public AuthContext(IDatabaseConfig config)
        {
            this.config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(config, DatabaseType.Auth);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountModel>(entity =>
            {
                entity.ToTable("account");

                entity.HasIndex(e => e.Email)
                    .HasName("email");

                entity.HasIndex(e => e.GameToken)
                    .HasName("gameToken");

                entity.HasIndex(e => e.SessionKey)
                    .HasName("sessionKey");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CreateTime)
                    .HasColumnName("createTime")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(128)")
                    .HasDefaultValue("");

                entity.Property(e => e.GameToken)
                    .IsRequired()
                    .HasColumnName("gameToken")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValue("");

                entity.Property(e => e.S)
                    .IsRequired()
                    .HasColumnName("s")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValue("");

                entity.Property(e => e.SessionKey)
                    .IsRequired()
                    .HasColumnName("sessionKey")
                    .HasColumnType("varchar(32)")
                    .HasDefaultValue("");

                entity.Property(e => e.V)
                    .IsRequired()
                    .HasColumnName("v")
                    .HasColumnType("varchar(512)")
                    .HasDefaultValue("");
            });

            modelBuilder.Entity<AccountCostumeUnlockModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.ItemId })
                    .HasName("PRIMARY");

                entity.ToTable("account_costume_unlock");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountCostumeUnlock)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_costume_item_id__account_id");
            });

            modelBuilder.Entity<AccountCurrencyModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CurrencyId })
                    .HasName("PRIMARY");

                entity.ToTable("account_currency");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.CurrencyId)
                    .HasColumnName("currencyId")
                    .HasColumnType("tinyint(4) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountCurrency)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_currency_id__account_id");
            });

            modelBuilder.Entity<AccountEntitlementModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.EntitlementId })
                    .HasName("PRIMARY");

                entity.ToTable("account_entitlement");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.EntitlementId)
                    .HasColumnName("entitlementId")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountEntitlement)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_entitlement_id__account_id");
            });

            modelBuilder.Entity<AccountGenericUnlockModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Entry })
                    .HasName("PRIMARY");

                entity.ToTable("account_generic_unlock");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Entry)
                    .HasColumnName("entry")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("current_timestamp()");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountGenericUnlock)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_generic_unlock_id__account_id");
            });

            modelBuilder.Entity<AccountKeybindingModel>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.InputActionId })
                    .HasName("PRIMARY");

                entity.ToTable("account_keybinding");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.InputActionId)
                    .HasColumnName("inputActionId")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(0);

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

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountKeybinding)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__account_keybinding_id__account_id");
            });

            modelBuilder.Entity<ServerModel>(entity =>
            {
                entity.ToTable("server");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("tinyint(3) unsigned")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasColumnName("host")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValue("127.0.0.1");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValue("NexusForever");

                entity.Property(e => e.Port)
                    .HasColumnName("port")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValue(24000);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0);

                entity.HasData(new ServerModel
                {
                    Id   = 1,
                    Host = "127.0.0.1",
                    Name = "NexusForever",
                    Port = 24000,
                    Type = 0
                });
            });

            modelBuilder.Entity<ServerMessageModel>(entity =>
            {
                entity.HasKey(e => new { e.Index, e.Language })
                    .HasName("PRIMARY");

                entity.ToTable("server_message");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Language)
                    .HasColumnName("language")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValue(0)
                    .ValueGeneratedNever();

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnName("message")
                    .HasColumnType("varchar(256)")
                    .HasDefaultValue("");

                entity.HasData(
                    new ServerMessageModel
                    {
                        Index    = 0,
                        Language = 0,
                        Message  = "Welcome to this NexusForever server!\nVisit: https://github.com/NexusForever/NexusForever"
                    },
                    new ServerMessageModel
                    {
                        Index    = 0,
                        Language = 1,
                        Message  = "Willkommen auf diesem NexusForever server!\nBesuch: https://github.com/NexusForever/NexusForever"
                    });
            });
        }
    }
}
