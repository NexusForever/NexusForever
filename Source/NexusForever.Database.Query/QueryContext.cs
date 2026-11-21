using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Query.Model;

namespace NexusForever.Database.Query
{
    public class QueryContext : DbContext
    {
        public DbSet<CharacterModel> Character { get; set; }

        public QueryContext(
            DbContextOptions<QueryContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterModel>(entity =>
            {
                entity.ToTable("character");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasIndex(c => c.Name);

                entity.HasIndex(c => c.Race);

                entity.HasIndex(c => c.Class);

                entity.HasIndex(c => c.Path);

                entity.HasIndex(c => c.CurrentRealmId);

                entity.HasIndex(c => c.WorldZoneId);

                entity.HasIndex(c => c.Level);

                entity.HasIndex(c => c.GuildName);

                entity.Property(c => c.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(c => c.RealmId)
                    .HasColumnName("realmId");

                entity.Property(c => c.Name)
                    .HasColumnName("name");

                entity.Property(c => c.RealmName)
                    .HasColumnName("realmName");

                entity.Property(c  => c.Race)
                    .HasColumnName("race");

                entity.Property(c => c.Class)
                    .HasColumnName("class");

                entity.Property(c => c.Path)
                    .HasColumnName("path");

                entity.Property(c => c.Faction)
                    .HasColumnName("faction");

                entity.Property(c => c.Sex)
                    .HasColumnName("sex");

                entity.Property(c => c.CurrentRealmId)
                    .HasColumnName("currentRealmId");

                entity.Property(c => c.WorldZoneId)
                    .HasColumnName("worldZoneId");

                entity.Property(c => c.Level)
                    .HasColumnName("level");

                entity.Property(c => c.GuildName)
                    .HasColumnName("guildName");

                entity.Property(c => c.LastOnline)
                    .HasColumnName("lastOnline");
            });
        }
    }
}
