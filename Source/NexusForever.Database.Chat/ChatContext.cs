using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Chat.Model;

namespace NexusForever.Database.Chat
{
    public class ChatContext : DbContext
    {
        public DbSet<CharacterModel> Character { get; set; }
        public DbSet<CharacterChatChannelModel> CharacterGroup { get; set; }
        public DbSet<ChatChannelModel> ChatChannel { get; set; }
        public DbSet<ChatChannelMemberModel> ChatChannelMember { get; set; }
        public DbSet<InternalMessageModel> InternalMessage { get; set; }

        public ChatContext(
            DbContextOptions<ChatContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterModel>(entity =>
            {
                entity.ToTable("character");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.Name)
                    .HasColumnName("name");

                entity.Property(entity => entity.RealmName)
                    .HasColumnName("realmName");

                entity.Property(entity => entity.Faction)
                    .HasColumnName("faction");

                entity.Property(entity => entity.IsOnline)
                    .HasColumnName("isOnline");
            });

            modelBuilder.Entity<CharacterChatChannelModel>(entity =>
            {
                entity.ToTable("character_chat_channel");

                entity.HasKey(entity => new { entity.CharacterId, entity.RealmId, entity.ChatId });

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.ChatId)
                    .HasColumnName("chatId");

                entity.HasOne(entity => entity.Character)
                    .WithMany(character => character.Channels)
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });

                entity.HasOne(entity => entity.Member)
                    .WithOne()
                    .HasForeignKey<CharacterChatChannelModel>(entity => new { entity.ChatId, entity.CharacterId, entity.RealmId });
            });

            modelBuilder.Entity<ChatChannelModel>(entity =>
            {
                entity.ToTable("chat_channel");

                entity.HasKey(entity => entity.ChatId);

                entity.HasIndex(entity => new { entity.Type, entity.Name })
                    .IsUnique();

                entity.HasIndex(entity => new { entity.Type, entity.ReferenceType, entity.ReferenceValue })
                    .IsUnique();

                entity.Property(entity => entity.ChatId)
                    .HasColumnName("chatId");

                entity.Property(entity => entity.Type)
                    .HasColumnName("type");

                entity.Property(entity => entity.Name)
                    .HasColumnName("name");

                entity.Property(entity => entity.Password)
                    .HasColumnName("password");

                entity.Property(entity => entity.ReferenceType)
                    .HasColumnName("referenceType");

                entity.Property(entity => entity.ReferenceValue)
                    .HasColumnName("referenceValue");
            });

            modelBuilder.Entity<ChatChannelMemberModel>(entity =>
            {
                entity.ToTable("chat_channel_member");

                entity.HasKey(entity => new { entity.ChatId, entity.CharacterId, entity.RealmId });

                entity.Property(entity => entity.ChatId)
                    .HasColumnName("chatId");

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.Property(entity => entity.Flags)
                    .HasColumnName("flags");

                entity.HasOne(entity => entity.ChatChannel)
                    .WithMany(channel => channel.Members)
                    .HasForeignKey(entity => entity.ChatId);

                entity.HasOne(entity => entity.Character)
                    .WithMany()
                    .HasForeignKey(entity => new { entity.CharacterId, entity.RealmId });
            });

            modelBuilder.Entity<ChatChannelOwnerModel>(entity =>
            {
                entity.ToTable("chat_channel_owner");

                entity.HasKey(entity => entity.ChatId);

                entity.Property(entity => entity.ChatId)
                    .HasColumnName("chatId");

                entity.Property(entity => entity.CharacterId)
                    .HasColumnName("characterId");

                entity.Property(entity => entity.RealmId)
                    .HasColumnName("realmId");

                entity.HasOne(entity => entity.ChatChannel)
                    .WithOne(channel => channel.Owner)
                    .HasForeignKey<ChatChannelOwnerModel>(entity => entity.ChatId);

                entity.HasOne(entity => entity.Member)
                    .WithOne()
                    .HasForeignKey<ChatChannelOwnerModel>(entity => new { entity.ChatId, entity.CharacterId, entity.RealmId });
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
