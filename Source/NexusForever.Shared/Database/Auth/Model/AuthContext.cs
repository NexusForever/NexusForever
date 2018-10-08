using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
        public virtual DbSet<Server> Server { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseMySql($"server={DatabaseManager.Config.Auth.Host};port={DatabaseManager.Config.Auth.Port};user={DatabaseManager.Config.Auth.Username};"
                    + $"password={DatabaseManager.Config.Auth.Password};database={DatabaseManager.Config.Auth.Database}");
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
        }
    }
}
