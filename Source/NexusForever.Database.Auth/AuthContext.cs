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

        private readonly IDatabaseConfiguration config;

        public AuthContext(IDatabaseConfiguration config)
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

                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.GameToken);
                entity.HasIndex(e => e.SessionKey);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.GameToken)
                    .HasMaxLength(32);

                entity.Property(e => e.S)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.SessionKey)
                    .HasMaxLength(32);

                entity.Property(e => e.V)
                    .IsRequired()
                    .HasMaxLength(512);
            });

            modelBuilder.Entity<AccountCostumeUnlockModel>(entity =>
            {
                entity.ToTable("account_costume_unlock");

                entity.HasKey(e => new { e.Id, e.ItemId });

                entity.HasOne(e => e.Account)
                    .WithMany(e => e.CostumeUnlocks)
                    .HasForeignKey(e => e.Id);

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<AccountCurrencyModel>(entity =>
            {
                entity.ToTable("account_currency");

                entity.HasKey(e => new { e.Id, e.CurrencyId });

                entity.HasOne(e => e.Account)
                    .WithMany(e => e.Currencies)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<AccountEntitlementModel>(entity =>
            {
                entity.ToTable("account_entitlement");

                entity.HasKey(e => new { e.Id, e.EntitlementId });

                entity.HasOne(e => e.Account)
                    .WithMany(e => e.Entitlements)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<AccountGenericUnlockModel>(entity =>
            {
                entity.ToTable("account_generic_unlock");

                entity.HasKey(e => new { e.Id, e.Entry });

                entity.HasOne(e => e.Account)
                    .WithMany(e => e.GenericUnlocks)
                    .HasForeignKey(e => e.Id);

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<AccountKeybindingModel>(entity =>
            {
                entity.ToTable("account_keybinding");

                entity.HasKey(e => new { e.Id, e.InputActionId });

                entity.HasOne(e => e.Account)
                    .WithMany(e => e.Keybindings)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<ServerModel>(entity =>
            {
                entity.ToTable("server");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(64);

                // default server entry
                entity.HasData(new ServerModel
                {
                    Id   = 1,
                    Name = "NexusForever",
                    Host = "127.0.0.1",
                    Port = 24000,
                    Type = 0
                });
            });

            modelBuilder.Entity<ServerMessageModel>(entity =>
            {
                entity.ToTable("server_message");

                entity.HasKey(e => new { e.Index, e.Language });

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(256);
            });
        }
    }
}
