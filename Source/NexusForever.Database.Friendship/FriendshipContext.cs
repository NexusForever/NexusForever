using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Friendship.Model;

namespace NexusForever.Database.Friendship
{
    public class FriendshipContext : DbContext
    {
        public DbSet<AccountModel> Account { get; set; }
        public DbSet<AccountFriendModel> AccountFriend { get; set; }
        public DbSet<AccountFriendInverseModel> AccountFriendInverse { get; set; }
        public DbSet<AccountFriendInviteModel> AccountFriendInvite { get; set; }
        public DbSet<AccountFriendInvitePendingModel> AccountFriendInvitePending { get; set; }

        public DbSet<CharacterModel> Character { get; set; }
        public DbSet<CharacterFriendModel> CharacterFriend { get; set; }
        public DbSet<CharacterFriendInverseModel> CharacterFriendInverse { get; set; }
        public DbSet<CharacterFriendInviteModel> CharacterFriendInvite { get; set; }
        public DbSet<CharacterFriendInvitePendingModel> CharacterFriendInvitePending { get; set; }

        public DbSet<CharacterStatModel> CharacterStat { get; set; }

        public DbSet<FriendAccountModel> FriendAccount { get; set; }
        public DbSet<FriendAccountInviteModel> FriendAccountInvite { get; set; }
        public DbSet<FriendModel> Friend { get; set; }
        public DbSet<FriendInviteModel> FriendInvite { get; set; }

        public DbSet<InternalMessageModel> InternalMessage { get; set; }

        public FriendshipContext(
            DbContextOptions<FriendshipContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountModel>(entity =>
            {
                entity.ToTable("account");

                entity.HasKey(entity => entity.AccountId);

                entity.HasIndex(entity => entity.Email)
                    .IsUnique();

                entity.HasIndex(entity => entity.Nickname)
                    .IsUnique();

                entity.Property(entity => entity.AccountId)
                    .HasColumnName("accountId");

                entity.Property(entity => entity.Email)
                    .HasColumnName("email");

                entity.Property(entity => entity.Nickname)
                    .HasColumnName("nickname");

                entity.Property(entity => entity.Status)
                    .HasColumnName("status");

                entity.Property(entity => entity.Presence)
                    .HasColumnName("presence");

                entity.Property(entity => entity.BlockAccountFriendRequests)
                    .HasColumnName("blockAccountFriendRequests");

                entity.Property(entity => entity.InvitePrivilegesSuspended)
                    .HasColumnName("invitePrivilegesSuspended");

                entity.Property(entity => entity.ActiveCharacterId)
                    .HasColumnName("activeCharacterId");

                entity.Property(entity => entity.ActiveRealmId)
                    .HasColumnName("activeRealmId");

                entity.Property(entity => entity.LastOnline)
                    .HasColumnName("lastOnline");

                entity.HasOne(entity => entity.ActiveCharacter)
                    .WithOne()
                    .HasForeignKey<AccountModel>(entity => new { entity.ActiveCharacterId, entity.ActiveRealmId });
            });

            modelBuilder.Entity<AccountFriendModel>(entity =>
            {
                entity.ToTable("account_friend");

                entity.HasKey(entity => new { entity.AccountId, entity.FriendAccountId } );

                entity.HasIndex(entity => entity.AccountId);

                entity.HasIndex(entity => entity.FriendAccountId)
                    .IsUnique();

                entity.Property(entity => entity.AccountId)
                    .HasColumnName("accountId");

                entity.Property(entity => entity.FriendAccountId)
                    .HasColumnName("friendAccountId");

                entity.HasOne(entity => entity.Account)
                    .WithMany(entity => entity.Friends)
                    .HasForeignKey(entity => entity.AccountId);

                entity.HasOne(entity => entity.FriendAccount)
                    .WithOne()
                    .HasForeignKey<AccountFriendModel>(entity => entity.FriendAccountId);
            });

            modelBuilder.Entity<AccountFriendInverseModel>(entity =>
            {
                entity.ToTable("account_friend_inverse");

                entity.HasKey(entity => new { entity.AccountId, entity.FriendAccountId });

                entity.HasIndex(entity => entity.AccountId);

                entity.HasIndex(entity => entity.FriendAccountId)
                    .IsUnique();

                entity.Property(entity => entity.AccountId)
                    .HasColumnName("accountId");

                entity.Property(entity => entity.FriendAccountId)
                    .HasColumnName("friendAccountId");

                entity.HasOne(entity => entity.Account)
                    .WithMany(entity => entity.FriendsInverse)
                    .HasForeignKey(entity => entity.AccountId);

                entity.HasOne(entity => entity.FriendAccount)
                    .WithOne()
                    .HasForeignKey<AccountFriendInverseModel>(entity => entity.FriendAccountId);
            });

            modelBuilder.Entity<AccountFriendInviteModel>(entity =>
            {
                entity.ToTable("account_friend_invite");

                entity.HasKey(entity => new { entity.AccountId, entity.FriendAccountInviteId });

                entity.HasIndex(entity => entity.AccountId);

                entity.HasIndex(entity => entity.FriendAccountInviteId)
                    .IsUnique();

                entity.Property(entity => entity.AccountId)
                    .HasColumnName("accountId");

                entity.Property(entity => entity.FriendAccountInviteId)
                    .HasColumnName("friendAccountInviteId");

                entity.HasOne(entity => entity.Account)
                    .WithMany(entity => entity.FriendInvites)
                    .HasForeignKey(entity => entity.AccountId);

                entity.HasOne(entity => entity.FriendAccountInvite)
                    .WithOne()
                    .HasForeignKey<AccountFriendInviteModel>(entity => entity.FriendAccountInviteId);
            });

            modelBuilder.Entity<AccountFriendInvitePendingModel>(entity =>
            {
                entity.ToTable("account_friend_invite_pending");

                entity.HasKey(entity => new { entity.AccountId, entity.FriendAccountInviteId });

                entity.HasIndex(entity => entity.AccountId);

                entity.HasIndex(entity => entity.FriendAccountInviteId)
                    .IsUnique();

                entity.Property(entity => entity.AccountId)
                    .HasColumnName("accountId");

                entity.Property(entity => entity.FriendAccountInviteId)
                    .HasColumnName("friendAccountInviteId");

                entity.HasOne(entity => entity.Account)
                    .WithMany(entity => entity.FriendInvitesPending)
                    .HasForeignKey(entity => entity.AccountId);

                entity.HasOne(entity => entity.FriendAccountInvite)
                    .WithOne()
                    .HasForeignKey<AccountFriendInvitePendingModel>(entity => entity.FriendAccountInviteId);
            });

            modelBuilder.Entity<CharacterModel>(entity =>
            {
                entity.ToTable("character");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.RealmName)
                    .HasColumnName("realmName");

                entity.Property(entity => entity.Name)
                    .HasColumnName("name");

                entity.Property(entity => entity.Race)
                    .HasColumnName("race");

                entity.Property(entity => entity.Class)
                    .HasColumnName("class");

                entity.Property(entity => entity.Path)
                    .HasColumnName("path");

                entity.Property(entity => entity.Faction)
                    .HasColumnName("faction");

                entity.Property(entity => entity.WorldZoneId)
                    .HasColumnName("worldZoneId");

                entity.Property(entity => entity.WorldId)
                    .HasColumnName("worldId");

                entity.Property(entity => entity.LastOnline)
                    .HasColumnName("lastOnline");
            });

            modelBuilder.Entity<CharacterFriendModel>(entity =>
            {
                entity.ToTable("character_friend");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.FriendId });

                entity.HasIndex(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasIndex(entity => entity.FriendId)
                    .IsUnique();

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.FriendId)
                    .HasColumnName("friendId");

                entity.HasOne(entity => entity.Character)
                    .WithMany(entity => entity.Friends)
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasOne(entity => entity.Friend)
                    .WithOne()
                    .HasForeignKey<CharacterFriendModel>(entity => entity.FriendId);
            });

            modelBuilder.Entity<CharacterFriendInverseModel>(entity =>
            {
                entity.ToTable("character_friend_inverse");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.FriendId });

                entity.HasIndex(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasIndex(entity => entity.FriendId)
                    .IsUnique();

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.FriendId)
                    .HasColumnName("friendId");

                entity.HasOne(entity => entity.Character)
                    .WithMany(entity => entity.FriendsInverse)
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasOne(entity => entity.Friend)
                    .WithOne()
                    .HasForeignKey<CharacterFriendInverseModel>(entity => entity.FriendId);
            });

            modelBuilder.Entity<CharacterFriendInviteModel>(entity =>
            {
                entity.ToTable("character_friend_invite");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.FriendInviteId });

                entity.HasIndex(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasIndex(entity => entity.FriendInviteId)
                    .IsUnique();

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.FriendInviteId)
                    .HasColumnName("friendInviteId");

                entity.HasOne(entity => entity.Character)
                    .WithMany(entity => entity.FriendInvites)
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasOne(entity => entity.FriendInvite)
                    .WithOne()
                    .HasForeignKey<CharacterFriendInviteModel>(entity => entity.FriendInviteId);
            });

            modelBuilder.Entity<CharacterFriendInvitePendingModel>(entity =>
            {
                entity.ToTable("character_friend_invite_pending");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.FriendInviteId });

                entity.HasIndex(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasIndex(entity => entity.FriendInviteId)
                    .IsUnique();

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.FriendInviteId)
                    .HasColumnName("friendInviteId");

                entity.HasOne(entity => entity.Character)
                    .WithMany(entity => entity.FriendInvitesPending)
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasOne(entity => entity.FriendInvite)
                    .WithOne()
                    .HasForeignKey<CharacterFriendInvitePendingModel>(entity => entity.FriendInviteId);
            });

            modelBuilder.Entity<CharacterStatModel>(entity =>
            {
                entity.ToTable("character_stat");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.Stat });

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

            modelBuilder.Entity<FriendAccountInviteModel>(entity =>
            {
                entity.ToTable("friendship_account_invite");

                entity.HasKey(entity => entity.Id);

                entity.Property(entity => entity.Id)
                    .HasColumnName("id");

                entity.Property(entity => entity.InviteeAccountId)
                    .HasColumnName("inviteeAccountId");

                entity.Property(entity => entity.InviterAccountId)
                    .HasColumnName("inviterAccountId");

                entity.Property(entity => entity.Seen)
                    .HasColumnName("seen");

                entity.Property(entity => entity.Note)
                    .HasColumnName("note");

                entity.Property(entity => entity.Expiration)
                    .HasColumnName("expiration");

                entity.HasOne(entity => entity.InviteeAccount)
                    .WithMany()
                    .HasForeignKey(entity => entity.InviteeAccountId);

                entity.HasOne(entity => entity.InviterAccount)
                    .WithMany()
                    .HasForeignKey(entity => entity.InviterAccountId);
            });

            modelBuilder.Entity<FriendAccountModel>(entity =>
            {
                entity.ToTable("friendship_account");

                entity.HasKey(entity => entity.Id);

                entity.Property(entity => entity.Id)
                    .HasColumnName("id");

                entity.Property(entity => entity.InviterAccountId)
                    .HasColumnName("inviterAccountId");

                entity.Property(entity => entity.InviteeAccountId)
                    .HasColumnName("inviteeAccountId");

                entity.Property(entity => entity.Note)
                    .HasColumnName("note");

                entity.HasOne(entity => entity.InviterAccount)
                    .WithMany()
                    .HasForeignKey(entity => entity.InviterAccountId);

                entity.HasOne(entity => entity.InviteeAccount)
                    .WithMany()
                    .HasForeignKey(entity => entity.InviteeAccountId);
            });

            modelBuilder.Entity<FriendInviteModel>(entity =>
            {
                entity.ToTable("friendship_invite");

                entity.HasKey(entity => entity.Id);

                entity.Property(entity => entity.Id)
                    .HasColumnName("id");

                entity.Property(entity => entity.InviteeCharacterId)
                    .HasColumnName("inviteeCharacterId");

                entity.Property(entity => entity.InviteeRealmId)
                    .HasColumnName("inviteeRealmId");

                entity.Property(entity => entity.InviterCharacterId)
                    .HasColumnName("inviterCharacterId");

                entity.Property(entity => entity.InviterRealmId)
                    .HasColumnName("inviterRealmId");

                entity.Property(entity => entity.Seen)
                    .HasColumnName("seen");

                entity.Property(entity => entity.Note)
                    .HasColumnName("note");

                entity.Property(entity => entity.Expiration)
                    .HasColumnName("expiration");

                entity.HasOne(entity => entity.InviteeCharacter)
                    .WithMany()
                    .HasForeignKey(entity => new { entity.InviteeCharacterId, entity.InviteeRealmId });

                entity.HasOne(entity => entity.InviterCharacter)
                    .WithMany()
                    .HasForeignKey(entity => new { entity.InviterCharacterId, entity.InviterRealmId });
            });

            modelBuilder.Entity<FriendModel>(entity =>
            {
                entity.ToTable("friendship");

                entity.HasKey(entity => entity.Id);

                entity.HasIndex(entity => new { entity.InviterCharacterId, entity.InviterRealmId, entity.InviteeCharacterId, entity.InviteeRealmId })
                    .IsUnique();

                entity.HasIndex(entity => new { entity.InviteeCharacterId, entity.InviteeRealmId });

                entity.Property(entity => entity.Id)
                    .HasColumnName("id");

                entity.Property(entity => entity.InviterCharacterId)
                    .HasColumnName("inviterCharacterId");

                entity.Property(entity => entity.InviterRealmId)
                    .HasColumnName("inviterRealmId");

                entity.Property(entity => entity.InviteeCharacterId)
                    .HasColumnName("friendCharacterId");

                entity.Property(entity => entity.InviteeRealmId)
                    .HasColumnName("friendRealmId");

                entity.Property(entity => entity.Note)
                    .HasColumnName("note");

                entity.Property(entity => entity.Type)
                    .HasColumnName("type");

                entity.HasOne(entity => entity.InviterCharacter)
                    .WithMany()
                    .HasForeignKey(entity => new { entity.InviterCharacterId, entity.InviterRealmId });

                entity.HasOne(entity => entity.InviteeCharacter)
                    .WithMany()
                    .HasForeignKey(entity => new { entity.InviteeCharacterId, entity.InviteeRealmId });
            });
        }
    }
}
