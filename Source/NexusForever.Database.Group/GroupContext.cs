using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Group.Model;

namespace NexusForever.Database.Group
{
    public class GroupContext : DbContext
    {
        public DbSet<CharacterModel> Character { get; set; }
        public DbSet<CharacterGroupModel> CharacterGroup { get; set; }
        public DbSet<CharacterStatModel> CharacterStat { get; set; }
        public DbSet<CharacterPropertyModel> CharacterProperty { get; set; }
        public DbSet<GroupModel> Group { get; set; }
        public DbSet<GroupInviteModel> GroupInvite { get; set; }
        public DbSet<GroupLeaderModel> GroupLeader { get; set; }
        public DbSet<GroupMarkerModel> GroupMarker { get; set; }
        public DbSet<GroupMemberModel> GroupMember { get; set; }
        public DbSet<GroupRequestModel> GroupRequest { get; set; }
        public DbSet<InternalMessageModel> InternalMessage { get; set; }

        public GroupContext(
            DbContextOptions<GroupContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterModel>(entity =>
            {
                entity.ToTable("character");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId })
                    .HasName("PRIMARY");

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.RealmName)
                    .HasColumnName("realmName")
                    .IsRequired();

                entity.Property(entity => entity.Name)
                    .HasColumnName("name")
                    .IsRequired();

                entity.Property(entity => entity.Sex)
                    .HasColumnName("sex");

                entity.Property(entity => entity.Race)
                    .HasColumnName("race");

                entity.Property(entity => entity.Class)
                    .HasColumnName("class");

                entity.Property(entity => entity.Path)
                    .HasColumnName("path");

                entity.Property(entity => entity.Faction)
                    .HasColumnName("faction");

                entity.Property(entity => entity.CurrentRealm)
                    .HasColumnName("currentRealm");

                entity.Property(entity => entity.WorldZoneId)
                    .HasColumnName("worldZoneId");

                entity.Property(entity => entity.MapId)
                    .HasColumnName("mapId");

                entity.Property(entity => entity.PositionX)
                    .HasColumnName("positionX");

                entity.Property(entity => entity.PositionY)
                    .HasColumnName("positionY");

                entity.Property(entity => entity.PositionZ)
                    .HasColumnName("positionZ");

                entity.Property(entity => entity.StatsDirty)
                    .HasColumnName("statsDirty")
                    .HasDefaultValue(false);

                entity.Property(entity => entity.RealmDirty)
                    .HasColumnName("realmDirty")
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<CharacterGroupModel>(entity =>
            {
                entity.ToTable("character_group",
                    t => t.HasCheckConstraint("CK_character_group_onlytwogroups", "`index` < 2"));

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.GroupId });

                entity.HasIndex(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasIndex(entity => new { entity.CharacterId, entity.RealmId, entity.Index })
                    .IsUnique();

                entity.Property(entity => entity.GroupId)
                    .HasColumnName("groupId");

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.Index)
                    .HasColumnName("index");

                entity.HasOne(entity => entity.Character)
                    .WithMany(entity => entity.Groups)
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasOne(entity => entity.Member)
                    .WithOne()
                    .HasForeignKey<CharacterGroupModel>(entity => new { entity.GroupId, entity.CharacterId, entity.RealmId });
            });

            modelBuilder.Entity<CharacterStatModel>(entity =>
            {
                entity.ToTable("character_stat");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.Stat })
                    .HasName("PRIMARY");

                entity.HasIndex(entity => new { entity.CharacterId, entity.RealmId });

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.Stat)
                    .HasColumnName("stat");

                entity.Property(entity => entity.Value)
                    .HasColumnName("value");

                entity.HasOne(entity => entity.Character)
                    .WithMany(entity => entity.Stats)
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });
            });

            modelBuilder.Entity<CharacterPropertyModel>(entity =>
            {
                entity.ToTable("character_property");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.Property })
                    .HasName("PRIMARY");

                entity.HasIndex(entity => new { entity.CharacterId, entity.RealmId });

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.Property)
                    .HasColumnName("property");

                entity.Property(entity => entity.Value)
                    .HasColumnName("value");

                entity.HasOne(entity => entity.Character)
                    .WithMany(entity => entity.Properties)
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });
            });

            modelBuilder.Entity<GroupModel>(entity =>
            {
                entity.ToTable("group");

                entity.HasKey(entity => entity.GroupId)
                    .HasName("PRIMARY");

                entity.HasIndex(entity => new { entity.Match, entity.MatchTeam })
                    .IsUnique();

                entity.Property(entity => entity.GroupId)
                    .HasColumnName("groupId")
                    .ValueGeneratedOnAdd();

                entity.Property(entity => entity.Flags)
                    .HasColumnName("flags");

                entity.Property(entity => entity.LootRule)
                    .HasColumnName("lootRule");

                entity.Property(entity => entity.LootRuleThreshold)
                    .HasColumnName("lootRuleThreshold");

                entity.Property(entity => entity.LootThreshold)
                    .HasColumnName("lootThreshold");

                entity.Property(entity => entity.LootRuleHarvest)
                    .HasColumnName("lootRuleHarvest");

                entity.Property(entity => entity.Match)
                    .HasColumnName("match");

                entity.Property(entity => entity.MatchTeam)
                    .HasColumnName("matchTeam");
            });

            modelBuilder.Entity<GroupInviteModel>(entity =>
            {
                entity.ToTable("group_invite");

                entity.HasKey(entity => new { entity.GroupId, entity.InviteeCharacterId, entity.InviteeRealmId })
                    .HasName("PRIMARY");

                entity.HasIndex(entity => entity.GroupId);

                entity.HasIndex(entity => new { entity.InviteeCharacterId, entity.InviteeRealmId })
                    .IsUnique();

                entity.Property(entity => entity.GroupId)
                    .HasColumnName("groupId");

                entity.Property(entity => entity.InviteeCharacterId)
                    .HasColumnName("inviteeCharacterId");

                entity.Property(entity => entity.InviteeRealmId)
                    .HasColumnName("inviteeRealmId");

                entity.Property(entity => entity.InviterRealmId)
                    .HasColumnName("inviterCharacterId");

                entity.Property(entity => entity.InviterRealmId)
                    .HasColumnName("inviterRealmId");

                entity.Property(entity => entity.Expiration)
                    .HasColumnName("expiration");

                entity.HasOne(entity => entity.Group)
                    .WithMany(entity => entity.Invites)
                    .HasForeignKey(entity => entity.GroupId);

                entity.HasOne(entity => entity.InviteeCharacter)
                    .WithOne(entity => entity.Invite)
                    .HasForeignKey<GroupInviteModel>(entity => new { entity.InviteeCharacterId, entity.InviteeRealmId })
                    .HasPrincipalKey<CharacterModel>(entity => new { entity.CharacterId, entity.RealmId });
            });

            modelBuilder.Entity<GroupLeaderModel>(entity =>
            {
                entity.ToTable("group_leader");

                entity.HasKey(entity => entity.GroupId)
                    .HasName("PRIMARY");

                entity.Property(entity => entity.GroupId)
                    .HasColumnName("groupId");

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.HasOne(entity => entity.Group)
                    .WithOne(entity => entity.Leader)
                    .HasForeignKey<GroupLeaderModel>(entity => entity.GroupId);

                entity.HasOne(entity => entity.Member)
                    .WithOne()
                    .HasForeignKey<GroupLeaderModel>(entity => new { entity.GroupId, entity.CharacterId, entity.RealmId })
                    .HasPrincipalKey<GroupMemberModel>(entity => new { entity.GroupId, entity.CharacterId, entity.RealmId });
            });

            modelBuilder.Entity<GroupMarkerModel>(entity =>
            {
                entity.ToTable("group_marker");

                entity.HasKey(entity => new { entity.GroupId, entity.Marker })
                    .HasName("PRIMARY");

                entity.HasIndex(entity => entity.GroupId);

                entity.HasIndex(entity => new { entity.GroupId, entity.UnitId })
                    .IsUnique();

                entity.Property(entity => entity.GroupId)
                    .HasColumnName("groupId");

                entity.Property(entity => entity.Marker)
                    .HasColumnName("marker");

                entity.Property(entity => entity.UnitId)
                    .HasColumnName("unitId");

                entity.HasOne(entity => entity.Group)
                    .WithMany(entity => entity.Markers)
                    .HasForeignKey(entity => entity.GroupId);
            });

            modelBuilder.Entity<GroupMemberModel>(entity =>
            {
                entity.ToTable("group_member");

                entity.HasKey(entity => new { entity.GroupId, entity.CharacterId, entity.RealmId })
                    .HasName("PRIMARY");

                entity.HasIndex(entity => entity.GroupId);

                entity.HasIndex(entity => new { entity.GroupId, entity.Index })
                    .IsUnique();

                entity.Property(entity => entity.GroupId)
                    .HasColumnName("groupId");

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.Index)
                    .HasColumnName("index");

                entity.Property(entity => entity.Flags)
                    .HasColumnName("flags");

                entity.Property(entity => entity.PositionDirty)
                    .HasColumnName("positionDirty")
                    .HasDefaultValue(false);

                entity.HasOne(entity => entity.Group)
                    .WithMany(entity => entity.Members)
                    .HasForeignKey(entity => entity.GroupId);

                entity.HasOne(entity => entity.Character)
                    .WithMany()
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });
            });

            modelBuilder.Entity<GroupRequestModel>(entity =>
            {
                entity.ToTable("group_request");

                entity.HasKey(entity => new { entity.GroupId })
                    .HasName("PRIMARY");

                entity.Property(entity => entity.GroupId)
                    .HasColumnName("groupId");

                entity.Property(entity => entity.RequesterCharacterId)
                    .HasColumnName("requesterCharacterId");

                entity.Property(entity => entity.RequesterRealmId)
                    .HasColumnName("requesterRealmId");

                entity.Property(entity => entity.RequesteeCharacterId)
                    .HasColumnName("requesteeCharacterId");

                entity.Property(entity => entity.RequesteeRealmId)
                    .HasColumnName("requesteeRealmId");

                entity.Property(entity => entity.RequestType)
                    .HasColumnName("requestType");

                entity.Property(entity => entity.Expiration)
                    .HasColumnName("expiration");

                entity.HasOne(entity => entity.Group)
                    .WithOne(entity => entity.Request)
                    .HasForeignKey<GroupRequestModel>(entity => entity.GroupId);

                entity.HasOne(entity => entity.Requester)
                    .WithOne()
                    .HasForeignKey<GroupRequestModel>(entity => new { entity.RequesterCharacterId, entity.RequesterRealmId });

                entity.HasOne(entity => entity.Requestee)
                    .WithOne()
                    .HasForeignKey<GroupRequestModel>(entity => new { entity.RequesteeCharacterId, entity.RequesteeRealmId });
            });

            modelBuilder.Entity<InternalMessageModel>(entity =>
            {
                entity.ToTable("internal_message");

                entity.HasKey(entity => entity.Id)
                    .HasName("PRIMARY");

                entity.HasIndex(entity => entity.ProcessedAt);

                entity.Property(entity => entity.Id)
                    .HasColumnName("messageId")
                    .IsRequired();

                entity.Property(entity => entity.CreatedAt)
                    .HasColumnName("createdAt")
                    .IsRequired();

                entity.Property(entity => entity.ProcessedAt)
                    .HasColumnName("processedAt");

                entity.Property(entity => entity.Type)
                    .HasColumnName("type")
                    .IsRequired();

                entity.Property(entity => entity.Payload)
                    .HasColumnName("data")
                    .IsRequired();
            });
        }
    }
}
