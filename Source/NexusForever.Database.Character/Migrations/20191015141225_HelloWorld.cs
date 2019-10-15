using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    public partial class HelloWorld : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<uint>(nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: true),
                    Sex = table.Column<byte>(nullable: false),
                    Race = table.Column<byte>(nullable: false),
                    Class = table.Column<byte>(nullable: false),
                    Level = table.Column<byte>(nullable: false),
                    FactionId = table.Column<ushort>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LocationX = table.Column<float>(nullable: false),
                    LocationY = table.Column<float>(nullable: false),
                    LocationZ = table.Column<float>(nullable: false),
                    WorldId = table.Column<ushort>(nullable: false),
                    WorldZoneId = table.Column<ushort>(nullable: false),
                    Title = table.Column<ushort>(nullable: false),
                    ActivePath = table.Column<uint>(nullable: false),
                    PathActivatedTimestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActiveCostumeIndex = table.Column<sbyte>(nullable: false),
                    InputKeySet = table.Column<sbyte>(nullable: false),
                    ActiveSpec = table.Column<byte>(nullable: false),
                    InnateIndex = table.Column<byte>(nullable: false),
                    TimePlayedTotal = table.Column<uint>(nullable: false),
                    TimePlayedLevel = table.Column<uint>(nullable: false),
                    DeleteTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    OriginalName = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "character_achievement",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    AchievementId = table.Column<ushort>(nullable: false),
                    Data0 = table.Column<uint>(nullable: false),
                    Data1 = table.Column<uint>(nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_achievement", x => new { x.Id, x.AchievementId });
                    table.ForeignKey(
                        name: "FK_character_achievement_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_action_set_amp",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    SpecIndex = table.Column<byte>(nullable: false),
                    AmpId = table.Column<ushort>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_action_set_amp", x => new { x.Id, x.SpecIndex, x.AmpId });
                    table.ForeignKey(
                        name: "FK_character_action_set_amp_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_action_set_shortcut",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    SpecIndex = table.Column<byte>(nullable: false),
                    Location = table.Column<ushort>(nullable: false),
                    ShortcutType = table.Column<byte>(nullable: false),
                    ObjectId = table.Column<uint>(nullable: false),
                    Tier = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_action_set_shortcut", x => new { x.Id, x.SpecIndex, x.Location });
                    table.ForeignKey(
                        name: "FK_character_action_set_shortcut_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_appearance",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Slot = table.Column<byte>(nullable: false),
                    DisplayId = table.Column<ushort>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_appearance", x => new { x.Id, x.Slot });
                    table.ForeignKey(
                        name: "FK_character_appearance_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_bone",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    BoneIndex = table.Column<byte>(nullable: false),
                    Bone = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_bone", x => new { x.Id, x.BoneIndex });
                    table.ForeignKey(
                        name: "FK_character_bone_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_costume",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Index = table.Column<byte>(nullable: false),
                    Mask = table.Column<uint>(nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_costume", x => new { x.Id, x.Index });
                    table.ForeignKey(
                        name: "FK_character_costume_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_currency",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    CurrencyId = table.Column<byte>(nullable: false),
                    Amount = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_currency", x => new { x.Id, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_character_currency_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_customisation",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Label = table.Column<uint>(nullable: false),
                    Value = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_customisation", x => new { x.Id, x.Label });
                    table.ForeignKey(
                        name: "FK_character_customisation_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_datacube",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    Datacube = table.Column<ushort>(nullable: false),
                    Progress = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_datacube", x => new { x.Id, x.Type, x.Datacube });
                    table.ForeignKey(
                        name: "FK_character_datacube_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_entitlement",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    EntitlementId = table.Column<byte>(nullable: false),
                    Amount = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_entitlement", x => new { x.Id, x.EntitlementId });
                    table.ForeignKey(
                        name: "FK_character_entitlement_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_keybinding",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    InputActionId = table.Column<ushort>(nullable: false),
                    DeviceEnum00 = table.Column<uint>(nullable: false),
                    DeviceEnum01 = table.Column<uint>(nullable: false),
                    DeviceEnum02 = table.Column<uint>(nullable: false),
                    Code00 = table.Column<uint>(nullable: false),
                    Code01 = table.Column<uint>(nullable: false),
                    Code02 = table.Column<uint>(nullable: false),
                    MetaKeys00 = table.Column<uint>(nullable: false),
                    MetaKeys01 = table.Column<uint>(nullable: false),
                    MetaKeys02 = table.Column<uint>(nullable: false),
                    EventTypeEnum00 = table.Column<uint>(nullable: false),
                    EventTypeEnum01 = table.Column<uint>(nullable: false),
                    EventTypeEnum02 = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_keybinding", x => new { x.Id, x.InputActionId });
                    table.ForeignKey(
                        name: "FK_character_keybinding_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_mail",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RecipientId = table.Column<ulong>(nullable: false),
                    SenderType = table.Column<byte>(nullable: false),
                    SenderId = table.Column<ulong>(nullable: false),
                    Subject = table.Column<string>(type: "varchar(200)", nullable: false),
                    Message = table.Column<string>(type: "varchar(2000)", nullable: false),
                    TextEntrySubject = table.Column<uint>(nullable: false),
                    TextEntryMessage = table.Column<uint>(nullable: false),
                    CreatureId = table.Column<uint>(nullable: false),
                    CurrencyType = table.Column<byte>(nullable: false),
                    CurrencyAmount = table.Column<ulong>(nullable: false),
                    IsCashOnDelivery = table.Column<byte>(nullable: false),
                    HasPaidOrCollectedCurrency = table.Column<byte>(nullable: false),
                    Flags = table.Column<byte>(nullable: false),
                    DeliveryTime = table.Column<byte>(nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_mail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_character_mail_character_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_path",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Path = table.Column<byte>(nullable: false),
                    Unlocked = table.Column<byte>(nullable: false),
                    TotalXp = table.Column<uint>(nullable: false),
                    LevelRewarded = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_path", x => new { x.Id, x.Path });
                    table.ForeignKey(
                        name: "FK_character_path_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_pet_customisation",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    ObjectId = table.Column<uint>(nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", nullable: false, defaultValue: ""),
                    FlairIdMask = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_pet_customisation", x => new { x.Id, x.Type, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_character_pet_customisation_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_pet_flair",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    PetFlairId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_pet_flair", x => new { x.Id, x.PetFlairId });
                    table.ForeignKey(
                        name: "FK_character_pet_flair_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_quest",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    QuestId = table.Column<ushort>(nullable: false),
                    State = table.Column<byte>(nullable: false),
                    Flags = table.Column<byte>(nullable: false),
                    Timer = table.Column<uint>(nullable: true),
                    Reset = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_quest", x => new { x.Id, x.QuestId });
                    table.ForeignKey(
                        name: "FK_character_quest_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_spell",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Spell4BaseId = table.Column<uint>(nullable: false),
                    Tier = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_spell", x => new { x.Id, x.Spell4BaseId });
                    table.ForeignKey(
                        name: "FK_character_spell_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_stats",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Stat = table.Column<byte>(nullable: false),
                    Value = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_stats", x => new { x.Id, x.Stat });
                    table.ForeignKey(
                        name: "FK_character_stats_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_title",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Title = table.Column<ushort>(nullable: false),
                    TimeRemaining = table.Column<uint>(nullable: false),
                    Revoked = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_title", x => new { x.Id, x.Title });
                    table.ForeignKey(
                        name: "FK_character_title_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_zonemap_hexgroup",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    ZoneMap = table.Column<ushort>(nullable: false),
                    HexGroup = table.Column<ushort>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_zonemap_hexgroup", x => new { x.Id, x.ZoneMap, x.HexGroup });
                    table.ForeignKey(
                        name: "FK_character_zonemap_hexgroup_character_Id",
                        column: x => x.Id,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<ulong>(nullable: true),
                    ItemId = table.Column<uint>(nullable: false),
                    Location = table.Column<ushort>(nullable: false),
                    BagIndex = table.Column<uint>(nullable: false),
                    StackCount = table.Column<uint>(nullable: false),
                    Charges = table.Column<uint>(nullable: false),
                    Durability = table.Column<float>(nullable: false),
                    ExpirationTimeLeft = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_item_character_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "residence",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<ulong>(nullable: false),
                    PropertyInfoId = table.Column<byte>(nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    PrivacyLevel = table.Column<byte>(nullable: false),
                    WallpaperId = table.Column<ushort>(nullable: false),
                    RoofDecorInfoId = table.Column<ushort>(nullable: false),
                    EntrywayDecorInfoId = table.Column<ushort>(nullable: false),
                    DoorDecorInfoId = table.Column<ushort>(nullable: false),
                    GroundWallpaperId = table.Column<ushort>(nullable: false),
                    MusicId = table.Column<ushort>(nullable: false),
                    SkyWallpaperId = table.Column<ushort>(nullable: false),
                    Flags = table.Column<ushort>(nullable: false),
                    ResourceSharing = table.Column<byte>(nullable: false),
                    GardenSharing = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_residence", x => x.Id);
                    table.ForeignKey(
                        name: "FK_residence_character_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_costume_item",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Index = table.Column<byte>(nullable: false),
                    Slot = table.Column<byte>(nullable: false),
                    ItemId = table.Column<uint>(nullable: false),
                    DyeData = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_costume_item", x => new { x.Id, x.Index, x.Slot });
                    table.ForeignKey(
                        name: "FK_character_costume_item_character_costume_Id_Index",
                        columns: x => new { x.Id, x.Index },
                        principalTable: "character_costume",
                        principalColumns: new[] { "Id", "Index" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_quest_objective",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    QuestId = table.Column<ushort>(nullable: false),
                    Index = table.Column<byte>(nullable: false),
                    Progress = table.Column<uint>(nullable: false),
                    Timer = table.Column<uint>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_quest_objective", x => new { x.Id, x.QuestId, x.Index });
                    table.ForeignKey(
                        name: "FK_character_quest_objective_character_quest_Id_QuestId",
                        columns: x => new { x.Id, x.QuestId },
                        principalTable: "character_quest",
                        principalColumns: new[] { "Id", "QuestId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_mail_attachment",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Index = table.Column<uint>(nullable: false),
                    ItemGuid = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_mail_attachment", x => new { x.Id, x.Index });
                    table.ForeignKey(
                        name: "FK_character_mail_attachment_character_mail_Id",
                        column: x => x.Id,
                        principalTable: "character_mail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_mail_attachment_item_ItemGuid",
                        column: x => x.ItemGuid,
                        principalTable: "item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "residence_decor",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    DecorId = table.Column<ulong>(nullable: false),
                    DecorInfoId = table.Column<uint>(nullable: false),
                    DecorType = table.Column<uint>(nullable: false),
                    Scale = table.Column<float>(nullable: false),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    Qx = table.Column<float>(nullable: false),
                    Qy = table.Column<float>(nullable: false),
                    Qz = table.Column<float>(nullable: false),
                    Qw = table.Column<float>(nullable: false),
                    DecorParentId = table.Column<ulong>(nullable: false),
                    ColourShiftId = table.Column<ushort>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_residence_decor", x => new { x.Id, x.DecorId });
                    table.ForeignKey(
                        name: "FK_residence_decor_residence_Id",
                        column: x => x.Id,
                        principalTable: "residence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "residence_plot",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false),
                    Index = table.Column<byte>(nullable: false),
                    PlotInfoId = table.Column<ushort>(nullable: false),
                    PlugItemId = table.Column<ushort>(nullable: false),
                    PlugFacing = table.Column<byte>(nullable: false),
                    BuildState = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_residence_plot", x => new { x.Id, x.Index });
                    table.ForeignKey(
                        name: "FK_residence_plot_residence_Id",
                        column: x => x.Id,
                        principalTable: "residence",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_character_AccountId",
                table: "character",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_character_mail_RecipientId",
                table: "character_mail",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_character_mail_attachment_Id",
                table: "character_mail_attachment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_character_mail_attachment_ItemGuid",
                table: "character_mail_attachment",
                column: "ItemGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_item_OwnerId",
                table: "item",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_residence_OwnerId",
                table: "residence",
                column: "OwnerId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_achievement");

            migrationBuilder.DropTable(
                name: "character_action_set_amp");

            migrationBuilder.DropTable(
                name: "character_action_set_shortcut");

            migrationBuilder.DropTable(
                name: "character_appearance");

            migrationBuilder.DropTable(
                name: "character_bone");

            migrationBuilder.DropTable(
                name: "character_costume_item");

            migrationBuilder.DropTable(
                name: "character_currency");

            migrationBuilder.DropTable(
                name: "character_customisation");

            migrationBuilder.DropTable(
                name: "character_datacube");

            migrationBuilder.DropTable(
                name: "character_entitlement");

            migrationBuilder.DropTable(
                name: "character_keybinding");

            migrationBuilder.DropTable(
                name: "character_mail_attachment");

            migrationBuilder.DropTable(
                name: "character_path");

            migrationBuilder.DropTable(
                name: "character_pet_customisation");

            migrationBuilder.DropTable(
                name: "character_pet_flair");

            migrationBuilder.DropTable(
                name: "character_quest_objective");

            migrationBuilder.DropTable(
                name: "character_spell");

            migrationBuilder.DropTable(
                name: "character_stats");

            migrationBuilder.DropTable(
                name: "character_title");

            migrationBuilder.DropTable(
                name: "character_zonemap_hexgroup");

            migrationBuilder.DropTable(
                name: "residence_decor");

            migrationBuilder.DropTable(
                name: "residence_plot");

            migrationBuilder.DropTable(
                name: "character_costume");

            migrationBuilder.DropTable(
                name: "character_mail");

            migrationBuilder.DropTable(
                name: "item");

            migrationBuilder.DropTable(
                name: "character_quest");

            migrationBuilder.DropTable(
                name: "residence");

            migrationBuilder.DropTable(
                name: "character");
        }
    }
}
