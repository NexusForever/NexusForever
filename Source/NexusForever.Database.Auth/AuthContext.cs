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
        public DbSet<AccountPermissionModel> AccountPermission { get; set; }
        public DbSet<AccountRoleModel> AccountRole { get; set; }
        public DbSet<PermissionModel> Permission { get; set; }
        public DbSet<RoleModel> Role { get; set; }
        public DbSet<RolePermissionModel> RolePermission { get; set; }
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

            optionsBuilder.EnableSensitiveDataLogging();
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

            modelBuilder.Entity<AccountPermissionModel>(entity =>
            {
                entity.ToTable("account_permission");

                entity.HasKey(i => new { i.Id, i.PermissionId });

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.PermissionId)
                    .HasColumnName("permissionId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(e => e.Account)
                    .WithMany(f => f.AccountPermission)
                    .HasForeignKey(e => e.Id)
                    .HasConstraintName("FK__account_permission_id__account_id");

                entity.HasOne(e => e.Permission)
                    .WithMany(f => f.AccountPermission)
                    .HasForeignKey(e => e.PermissionId)
                    .HasConstraintName("FK__account_permission_permission_id__permission_id");
            });

            modelBuilder.Entity<AccountRoleModel>(entity =>
            {
                entity.ToTable("account_role");

                entity.HasKey(i => new { i.Id, i.RoleId });

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.RoleId)
                    .HasColumnName("roleId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(e => e.Account)
                    .WithMany(f => f.AccountRole)
                    .HasForeignKey(e => e.Id)
                    .HasConstraintName("FK__account_role_id__account_id");

                entity.HasOne(e => e.Role)
                    .WithMany(f => f.AccountRole)
                    .HasForeignKey(e => e.RoleId)
                    .HasConstraintName("FK__account_role_role_id__role_id");
            });

            modelBuilder.Entity<PermissionModel>(entity =>
            {
                entity.ToTable("permission");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValue("");

                entity.HasData(
                    new PermissionModel
                    {
                        Id   = 1,
                        Name = "Category: Account"
                    },
                    new PermissionModel
                    {
                        Id   = 2,
                        Name = "Command: AccountCreate"
                    },
                    new PermissionModel
                    {
                        Id   = 3,
                        Name = "Command: AccountDelete"
                    },
                    new PermissionModel
                    {
                        Id   = 15,
                        Name = "Category: Achievement"
                    },
                    new PermissionModel
                    {
                        Id   = 17,
                        Name = "Command: AchievementUpdate"
                    },
                    new PermissionModel
                    {
                        Id   = 16,
                        Name = "Command: AchievementGrant"
                    },
                    new PermissionModel
                    {
                        Id   = 18,
                        Name = "Category: Broadcast"
                    },
                    new PermissionModel
                    {
                        Id   = 19,
                        Name = "Command: BroadcastMessage"
                    },
                    new PermissionModel
                    {
                        Id   = 20,
                        Name = "Category: Character"
                    },
                    new PermissionModel
                    {
                        Id   = 21,
                        Name = "Command: CharacterXP"
                    },
                    new PermissionModel
                    {
                        Id   = 22,
                        Name = "Command: CharacterLevel"
                    },
                    new PermissionModel
                    {
                        Id   = 5,
                        Name = "Command: CharacterSave"
                    },
                    new PermissionModel
                    {
                        Id   = 23,
                        Name = "Category: Currency"
                    },
                    new PermissionModel
                    {
                        Id   = 24,
                        Name = "Category: CurrencyAccount"
                    },
                    new PermissionModel
                    {
                        Id   = 25,
                        Name = "Command: CurrencyAccountAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 26,
                        Name = "Command: CurrencyAccountList"
                    },
                    new PermissionModel
                    {
                        Id   = 27,
                        Name = "Category: CurrencyCharacter"
                    },
                    new PermissionModel
                    {
                        Id   = 28,
                        Name = "Command: CurrencyCharacterAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 29,
                        Name = "Command: CurrencyCharacterList"
                    },
                    new PermissionModel
                    {
                        Id   = 30,
                        Name = "Category: Disable"
                    },
                    new PermissionModel
                    {
                        Id   = 31,
                        Name = "Command: DisableInfo"
                    },
                    new PermissionModel
                    {
                        Id   = 32,
                        Name = "Command: DisableReload"
                    },
                    new PermissionModel
                    {
                        Id   = 33,
                        Name = "Category: Door"
                    },
                    new PermissionModel
                    {
                        Id   = 34,
                        Name = "Command: DoorOpen"
                    },
                    new PermissionModel
                    {
                        Id   = 35,
                        Name = "Command: DoorClose"
                    },
                    new PermissionModel
                    {
                        Id   = 36,
                        Name = "Category: Entitlement"
                    },
                    new PermissionModel
                    {
                        Id   = 40,
                        Name = "Category: EntitlementAccount"
                    },
                    new PermissionModel
                    {
                        Id   = 41,
                        Name = "Command: EntitlementAccountAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 42,
                        Name = "Command: EntitlementAccountList"
                    },
                    new PermissionModel
                    {
                        Id   = 37,
                        Name = "Category: EntitlementCharacter"
                    },
                    new PermissionModel
                    {
                        Id   = 38,
                        Name = "Command: EntitlementCharacterAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 39,
                        Name = "Command: EntitlementCharacterList"
                    },
                    new PermissionModel
                    {
                        Id   = 43,
                        Name = "Category: Entity"
                    },
                    new PermissionModel
                    {
                        Id   = 44,
                        Name = "Command: EntityInfo"
                    },
                    new PermissionModel
                    {
                        Id   = 45,
                        Name = "Command: EntityProperties"
                    },
                    new PermissionModel
                    {
                        Id   = 60,
                        Name = "Category: EntityModify"
                    },
                    new PermissionModel
                    {
                        Id   = 61,
                        Name = "Command: EntityModifyDisplayInfo"
                    },
                    new PermissionModel
                    {
                        Id   = 46,
                        Name = "Category: Generic"
                    },
                    new PermissionModel
                    {
                        Id   = 47,
                        Name = "Command: GenericUnlock"
                    },
                    new PermissionModel
                    {
                        Id   = 48,
                        Name = "Command: GenericUnlockAll"
                    },
                    new PermissionModel
                    {
                        Id   = 49,
                        Name = "Command: GenericList"
                    },
                    new PermissionModel
                    {
                        Id   = 4,
                        Name = "Category: Help"
                    },
                    new PermissionModel
                    {
                        Id   = 54,
                        Name = "Category: House"
                    },
                    new PermissionModel
                    {
                        Id   = 58,
                        Name = "Command: HouseTeleport"
                    },
                    new PermissionModel
                    {
                        Id   = 55,
                        Name = "Category: HouseDecor"
                    },
                    new PermissionModel
                    {
                        Id   = 56,
                        Name = "Command: HouseDecorAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 57,
                        Name = "Command: HouseDecorLookup"
                    },
                    new PermissionModel
                    {
                        Id   = 59,
                        Name = "Category: Item"
                    },
                    new PermissionModel
                    {
                        Id   = 6,
                        Name = "Command: ItemAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 62,
                        Name = "Category: Movement"
                    },
                    new PermissionModel
                    {
                        Id   = 63,
                        Name = "Category: MovementSpline"
                    },
                    new PermissionModel
                    {
                        Id   = 64,
                        Name = "Command: MovementSplineAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 65,
                        Name = "Command: MovementSplineClear"
                    },
                    new PermissionModel
                    {
                        Id   = 66,
                        Name = "Command: MovementSplineLaunch"
                    },
                    new PermissionModel
                    {
                        Id   = 67,
                        Name = "Category: MovementGenerator"
                    },
                    new PermissionModel
                    {
                        Id   = 68,
                        Name = "Command: MovementGeneratorDirect"
                    },
                    new PermissionModel
                    {
                        Id   = 69,
                        Name = "Command: MovementGeneratorRandom"
                    },
                    new PermissionModel
                    {
                        Id   = 70,
                        Name = "Category: Path"
                    },
                    new PermissionModel
                    {
                        Id   = 71,
                        Name = "Command: PathUnlock"
                    },
                    new PermissionModel
                    {
                        Id   = 72,
                        Name = "Command: PathActivate"
                    },
                    new PermissionModel
                    {
                        Id   = 73,
                        Name = "Command: PathXP"
                    },
                    new PermissionModel
                    {
                        Id   = 74,
                        Name = "Category: Pet"
                    },
                    new PermissionModel
                    {
                        Id   = 75,
                        Name = "Command: PetUnlockFlair"
                    },
                    new PermissionModel
                    {
                        Id   = 76,
                        Name = "Category: Quest"
                    },
                    new PermissionModel
                    {
                        Id   = 77,
                        Name = "Command: QuestAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 78,
                        Name = "Command: QuestAchieve"
                    },
                    new PermissionModel
                    {
                        Id   = 79,
                        Name = "Command: QuestAchieveObjective"
                    },
                    new PermissionModel
                    {
                        Id   = 80,
                        Name = "Command: QuestObjective"
                    },
                    new PermissionModel
                    {
                        Id   = 81,
                        Name = "Command: QuestKill"
                    },
                    new PermissionModel
                    {
                        Id   = 7,
                        Name = "Category: RBAC"
                    },
                    new PermissionModel
                    {
                        Id   = 8,
                        Name = "Category: RBACAccount"
                    },
                    new PermissionModel
                    {
                        Id   = 9,
                        Name = "Category: RBACAccountPermission"
                    },
                    new PermissionModel
                    {
                        Id   = 10,
                        Name = "Command: RBACAccountPermissionGrant"
                    },
                    new PermissionModel
                    {
                        Id   = 11,
                        Name = "Command: RBACAccountPermissionRevoke"
                    },
                    new PermissionModel
                    {
                        Id   = 12,
                        Name = "Category: RBACAccountRole"
                    },
                    new PermissionModel
                    {
                        Id   = 13,
                        Name = "Command: RBACAccountRoleGrant"
                    },
                    new PermissionModel
                    {
                        Id   = 14,
                        Name = "Command: RBACAccountRoleRevoke"
                    },
                    new PermissionModel
                    {
                        Id   = 83,
                        Name = "Category: Spell"
                    },
                    new PermissionModel
                    {
                        Id   = 84,
                        Name = "Command: SpellAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 85,
                        Name = "Command: SpellCast"
                    },
                    new PermissionModel
                    {
                        Id   = 86,
                        Name = "Command: SpellResetCooldown"
                    },
                    new PermissionModel
                    {
                        Id   = 50,
                        Name = "Category: Teleport"
                    },
                    new PermissionModel
                    {
                        Id   = 51,
                        Name = "Command: TeleportCoordinates"
                    },
                    new PermissionModel
                    {
                        Id   = 52,
                        Name = "Command: TeleportLocation"
                    },
                    new PermissionModel
                    {
                        Id   = 53,
                        Name = "Command: TeleportName"
                    },
                    new PermissionModel
                    {
                        Id   = 87,
                        Name = "Category: Title"
                    },
                    new PermissionModel
                    {
                        Id   = 88,
                        Name = "Command: TitleAdd"
                    },
                    new PermissionModel
                    {
                        Id   = 89,
                        Name = "Command: TitleRevoke"
                    },
                    new PermissionModel
                    {
                        Id   = 90,
                        Name = "Command: TitleAll"
                    },
                    new PermissionModel
                    {
                        Id   = 91,
                        Name = "Command: TitleNone"
                    },
                    new PermissionModel
                    {
                        Id   = 92,
                        Name = "Command: ItemLookup"
                    },
                    new PermissionModel
                    {
                        Id   = 82,
                        Name = "Category: Realm"
                    },
                    new PermissionModel
                    {
                        Id   = 94,
                        Name = "Command: RealmMOTD"
                    },
                    new PermissionModel
                    {
                        Id   = 95,
                        Name = "Category: Story"
                    },
                    new PermissionModel
                    {
                        Id   = 96,
                        Name = "Command: StoryPanel"
                    },
                    new PermissionModel
                    {
                        Id   = 97,
                        Name = "Command: StoryCommunicator"
                    },
                    new PermissionModel
                    {
                        Id   = 98,
                        Name = "Category: Reputation"
                    },
                    new PermissionModel
                    {
                        Id   = 99,
                        Name = "Command: ReputationUpdate"
                    },
                    new PermissionModel
                    {
                        Id   = 10000,
                        Name = "Other: InstantLogout"
                    },
                    new PermissionModel
                    {
                        Id   = 10001,
                        Name = "Other: Signature"
                    });
            });

            modelBuilder.Entity<RoleModel>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(64)")
                    .HasDefaultValue("");

                entity.Property(e => e.Flags)
                    .HasColumnName("flags")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasData(
                    new RoleModel
                    {
                        Id    = 1,
                        Name  = "Player",
                        Flags = 1
                    },
                    new RoleModel
                    {
                        Id    = 2,
                        Name  = "GameMaster",
                        Flags = 1
                    },
                    new RoleModel
                    {
                        Id    = 3,
                        Name  = "Administrator",
                        Flags = 2
                    },
                    new RoleModel
                    {
                        Id    = 4,
                        Name  = "Console",
                        Flags = 2
                    },
                    new RoleModel
                    {
                        Id    = 5,
                        Name  = "WebSocket",
                        Flags = 2
                    });
            });

            modelBuilder.Entity<RolePermissionModel>(entity =>
            {
                entity.ToTable("role_permission");

                entity.HasKey(i => new { i.Id, i.PermissionId });

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.Property(e => e.PermissionId)
                    .HasColumnName("permissionId")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValue(0);

                entity.HasOne(e => e.Role)
                    .WithMany(f => f.RolePermission)
                    .HasForeignKey(e => e.Id)
                    .HasConstraintName("FK__role_permission_id__role_id");

                entity.HasOne(e => e.Permission)
                    .WithMany(f => f.RolePermission)
                    .HasForeignKey(e => e.PermissionId)
                    .HasConstraintName("FK__role_permission_permission_id__permission_id");
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
