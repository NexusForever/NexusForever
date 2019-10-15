using Microsoft.EntityFrameworkCore;
using NexusForever.Database.Character.Model;
using NexusForever.Database.Configuration;

namespace NexusForever.Database.Character
{
    public class CharacterContext : DbContext
    {
        public DbSet<CharacterModel> Character { get; set; }
        public DbSet<CharacterAchievementModel> CharacterAchievement { get; set; }
        public DbSet<CharacterActionSetAmpModel> CharacterActionSetAmp { get; set; }
        public DbSet<CharacterActionSetShortcutModel> CharacterActionSetShortcut { get; set; }
        public DbSet<CharacterAppearanceModel> CharacterAppearance { get; set; }
        public DbSet<CharacterBoneModel> CharacterBone { get; set; }
        public DbSet<CharacterCostumeModel> CharacterCostume { get; set; }
        public DbSet<CharacterCostumeItemModel> CharacterCostumeItem { get; set; }
        public DbSet<CharacterCurrencyModel> CharacterCurrency { get; set; }
        public DbSet<CharacterCustomisationModel> CharacterCustomisation { get; set; }
        public DbSet<CharacterDatacubeModel> CharacterDatacube { get; set; }
        public DbSet<CharacterEntitlementModel> CharacterEntitlement { get; set; }
        public DbSet<CharacterKeybindingModel> CharacterKeybinding { get; set; }
        public DbSet<MailModel> CharacterMail { get; set; }
        public DbSet<MailAttachmentModel> CharacterMailAttachment { get; set; }
        public DbSet<CharacterPathModel> CharacterPath { get; set; }
        public DbSet<CharacterPetCustomisationModel> CharacterPetCustomisation { get; set; }
        public DbSet<CharacterPetFlairModel> CharacterPetFlair { get; set; }
        public DbSet<CharacterQuestModel> CharacterQuest { get; set; }
        public DbSet<CharacterQuestObjectiveModel> CharacterQuestObjective { get; set; }
        public DbSet<CharacterSpellModel> CharacterSpell { get; set; }
        public DbSet<CharacterStatModel> CharacterStats { get; set; }
        public DbSet<CharacterTitleModel> CharacterTitle { get; set; }
        public DbSet<CharacterZonemapHexgroupModel> CharacterZonemapHexgroup { get; set; }
        public DbSet<ItemModel> Item { get; set; }
        public DbSet<ResidenceModel> Residence { get; set; }
        public DbSet<ResidenceDecorModel> ResidenceDecor { get; set; }
        public DbSet<ResidencePlotModel> ResidencePlot { get; set; }

        private readonly IDatabaseConfiguration config;

        public CharacterContext(IDatabaseConfiguration config)
        {
            this.config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(config, DatabaseType.Character);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CharacterModel>(entity =>
            {
                entity.ToTable("character");

                entity.HasIndex(e => e.AccountId);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeleteTime)
                    .HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.OriginalName)
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.PathActivatedTimestamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<CharacterAchievementModel>(entity =>
            {
                entity.ToTable("character_achievement");

                entity.HasKey(e => new { e.Id, e.AchievementId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Achievements)
                    .HasForeignKey(e => e.Id);

                entity.Property(e => e.DateCompleted)
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<CharacterActionSetAmpModel>(entity =>
            {
                entity.ToTable("character_action_set_amp");

                entity.HasKey(e => new { e.Id, e.SpecIndex, e.AmpId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.ActionSetAmps)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterActionSetShortcutModel>(entity =>
            {
                entity.ToTable("character_action_set_shortcut");

                entity.HasKey(e => new { e.Id, e.SpecIndex, e.Location });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.ActionSetShortcuts)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterAppearanceModel>(entity =>
            {
                entity.ToTable("character_appearance");

                entity.HasKey(e => new { e.Id, e.Slot });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Appearance)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterBoneModel>(entity =>
            {
                entity.ToTable("character_bone");

                entity.HasKey(e => new { e.Id, e.BoneIndex });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Bones)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterCostumeModel>(entity =>
            {
                entity.ToTable("character_costume");

                entity.HasKey(e => new { e.Id, e.Index });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Costumes)
                    .HasForeignKey(e => e.Id);

                modelBuilder.Entity<CharacterCostumeModel>()
                    .Property(p => p.Index)
                    .ValueGeneratedNever();

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<CharacterCostumeItemModel>(entity =>
            {
                entity.ToTable("character_costume_item");

                entity.HasKey(e => new { e.Id, e.Index, e.Slot });

                entity.HasOne(e => e.Costume)
                    .WithMany(e => e.Items)
                    .HasForeignKey(e => new { e.Id, e.Index });

                modelBuilder.Entity<CharacterCostumeItemModel>()
                    .Property(p => p.Slot)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<CharacterCurrencyModel>(entity =>
            {
                entity.ToTable("character_currency");

                entity.HasKey(e => new { e.Id, e.CurrencyId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Currencies)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterCustomisationModel>(entity =>
            {
                entity.ToTable("character_customisation");

                entity.HasKey(e => new { e.Id, e.Label });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Customisations)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterDatacubeModel>(entity =>
            {
                entity.ToTable("character_datacube");

                entity.HasKey(e => new { e.Id, e.Type, e.Datacube });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Datacubes)
                    .HasForeignKey(e => e.Id);

                modelBuilder.Entity<CharacterDatacubeModel>()
                    .Property(p => p.Type)
                    .ValueGeneratedNever();

                modelBuilder.Entity<CharacterDatacubeModel>()
                    .Property(p => p.Datacube)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<CharacterEntitlementModel>(entity =>
            {
                entity.ToTable("character_entitlement");

                entity.HasKey(e => new { e.Id, e.EntitlementId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Entitlements)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterKeybindingModel>(entity =>
            {
                entity.ToTable("character_keybinding");

                entity.HasKey(e => new { e.Id, e.InputActionId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Keybindings)
                    .HasForeignKey(e => e.Id);

                modelBuilder.Entity<CharacterKeybindingModel>()
                    .Property(p => p.InputActionId)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<MailModel>(entity =>
            {
                entity.ToTable("character_mail");

                entity.HasOne(e => e.Recipient)
                    .WithMany(e => e.Mail)
                    .HasForeignKey(e => e.RecipientId);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasColumnType("varchar(2000)");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<MailAttachmentModel>(entity =>
            {
                entity.ToTable("character_mail_attachment");

                entity.HasKey(e => new { e.Id, e.Index });

                entity.HasOne(e => e.Mail)
                    .WithMany(e => e.Attachments)
                    .HasForeignKey(e => e.Id);

                entity.HasOne(e => e.Item)
                    .WithOne(e => e.Attachment)
                    .HasForeignKey<MailAttachmentModel>(e => e.ItemGuid);

                modelBuilder.Entity<MailAttachmentModel>()
                    .Property(p => p.Index)
                    .ValueGeneratedNever();

                entity.HasIndex(e => e.ItemGuid)
                    .IsUnique();
            });

            modelBuilder.Entity<CharacterPathModel>(entity =>
            {
                entity.ToTable("character_path");

                entity.HasKey(e => new { e.Id, e.Path });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Paths)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterPetCustomisationModel>(entity =>
            {
                entity.ToTable("character_pet_customisation");

                entity.HasKey(e => new { e.Id, e.Type, e.ObjectId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.PetCustomisations)
                    .HasForeignKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(128)")
                    .HasDefaultValue("");
            });

            modelBuilder.Entity<CharacterPetFlairModel>(entity =>
            {
                entity.ToTable("character_pet_flair");

                entity.HasKey(e => new { e.Id, e.PetFlairId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.PetFlairs)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterQuestModel>(entity =>
            {
                entity.ToTable("character_quest");

                entity.HasKey(e => new { e.Id, e.QuestId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Quests)
                    .HasForeignKey(e => e.Id);

                entity.Property(e => e.Reset)
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<CharacterQuestObjectiveModel>(entity =>
            {
                entity.ToTable("character_quest_objective");

                entity.HasKey(e => new { e.Id, e.QuestId, e.Index });

                entity.HasOne(e => e.Quest)
                    .WithMany(e => e.Objectives)
                    .HasForeignKey(e => new { e.Id, e.QuestId });
            });

            modelBuilder.Entity<CharacterSpellModel>(entity =>
            {
                entity.ToTable("character_spell");

                entity.HasKey(e => new { e.Id, e.Spell4BaseId });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Spells)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterStatModel>(entity =>
            {
                entity.ToTable("character_stats");

                entity.HasKey(e => new { e.Id, e.Stat });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Stats)
                    .HasForeignKey(e => e.Id);

                modelBuilder.Entity<CharacterStatModel>()
                    .Property(p => p.Stat)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<CharacterTitleModel>(entity =>
            {
                entity.ToTable("character_title");

                entity.HasKey(e => new { e.Id, e.Title });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Titles)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<CharacterZonemapHexgroupModel>(entity =>
            {
                entity.ToTable("character_zonemap_hexgroup");

                entity.HasKey(e => new { e.Id, e.ZoneMap, e.HexGroup });

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.ZonemapHexgroups)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<ItemModel>(entity =>
            {
                entity.ToTable("item");

                entity.HasOne(e => e.Character)
                    .WithMany(e => e.Items)
                    .HasForeignKey(e => e.OwnerId);
            });

            modelBuilder.Entity<ResidenceModel>(entity =>
            {
                entity.ToTable("residence");

                entity.HasIndex(e => e.OwnerId)
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.HasOne(d => d.Owner)
                    .WithOne(p => p.Residence)
                    .HasForeignKey<ResidenceModel>(d => d.OwnerId);
            });

            modelBuilder.Entity<ResidenceDecorModel>(entity =>
            {
                entity.ToTable("residence_decor");

                entity.HasKey(e => new { e.Id, e.DecorId });

                entity.HasOne(e => e.Residence)
                    .WithMany(e => e.Decor)
                    .HasForeignKey(e => e.Id);
            });

            modelBuilder.Entity<ResidencePlotModel>(entity =>
            {
                entity.ToTable("residence_plot");

                entity.HasKey(e => new { e.Id, e.Index });

                entity.HasOne(e => e.Residence)
                    .WithMany(e => e.Plots)
                    .HasForeignKey(e => e.Id);
            });
        }
    }
}
