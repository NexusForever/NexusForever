using System;
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
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    accountId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    name = table.Column<string>(type: "varchar(50)", nullable: true, defaultValue: ""),
                    sex = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    race = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    @class = table.Column<byte>(name: "class", type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    level = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    factionId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()"),
                    lastOnline = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()"),
                    locationX = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    locationY = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    locationZ = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    worldId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    worldZoneId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    title = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    activePath = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    pathActivatedTimestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()"),
                    activeCostumeIndex = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValue: (sbyte)-1),
                    inputKeySet = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValue: (sbyte)0),
                    activeSpec = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    innateIndex = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    timePlayedTotal = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    timePlayedLevel = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    deleteTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    originalName = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "character_achievement",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    achievementId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    data0 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    data1 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    dateCompleted = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.achievementId });
                    table.ForeignKey(
                        name: "FK__character_achievement_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_action_set_amp",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    specIndex = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    ampId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.specIndex, x.ampId });
                    table.ForeignKey(
                        name: "FK__character_action_set_amp_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_action_set_shortcut",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    specIndex = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    location = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    shortcutType = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    objectId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    tier = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.specIndex, x.location });
                    table.ForeignKey(
                        name: "FK__character_action_set_shortcut_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_appearance",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    slot = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    displayId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.slot });
                    table.ForeignKey(
                        name: "FK__character_appearance_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_bone",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    boneIndex = table.Column<byte>(type: "tinyint(4) unsigned", nullable: false, defaultValue: (byte)0),
                    bone = table.Column<float>(type: "float", nullable: false, defaultValue: 0f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.boneIndex });
                    table.ForeignKey(
                        name: "FK_character_bone_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_costume",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    index = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    mask = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.index });
                    table.ForeignKey(
                        name: "FK__character_costume_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_currency",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    currencyId = table.Column<byte>(type: "tinyint(4) unsigned", nullable: false, defaultValue: (byte)0),
                    amount = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.currencyId });
                    table.ForeignKey(
                        name: "FK_character_currency_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_customisation",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    label = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    value = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.label });
                    table.ForeignKey(
                        name: "FK__character_customisation_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_datacube",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    type = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    datacube = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    progress = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.type, x.datacube });
                    table.ForeignKey(
                        name: "FK__character_datacube_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_entitlement",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    entitlementId = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    amount = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.entitlementId });
                    table.ForeignKey(
                        name: "FK__character_entitlement_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_keybinding",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    inputActionId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    deviceEnum00 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    deviceEnum01 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    deviceEnum02 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    code00 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    code01 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    code02 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    metaKeys00 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    metaKeys01 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    metaKeys02 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    eventTypeEnum00 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    eventTypeEnum01 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    eventTypeEnum02 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.inputActionId });
                    table.ForeignKey(
                        name: "FK__character_keybinding_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_mail",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    recipientId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    senderType = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    senderId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    subject = table.Column<string>(type: "varchar(200)", nullable: false, defaultValue: ""),
                    message = table.Column<string>(type: "varchar(2000)", nullable: false, defaultValue: ""),
                    textEntrySubject = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    textEntryMessage = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    creatureId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    currencyType = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    currencyAmount = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    isCashOnDelivery = table.Column<byte>(type: "tinyint(8) unsigned", nullable: false, defaultValue: (byte)0),
                    hasPaidOrCollectedCurrency = table.Column<byte>(type: "tinyint(8) unsigned", nullable: false, defaultValue: (byte)0),
                    flags = table.Column<byte>(type: "tinyint(8) unsigned", nullable: false, defaultValue: (byte)0),
                    deliveryTime = table.Column<byte>(type: "tinyint(8) unsigned", nullable: false, defaultValue: (byte)0),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_mail", x => x.id);
                    table.ForeignKey(
                        name: "FK__character_mail_recipientId__character_id",
                        column: x => x.recipientId,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_path",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    path = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    unlocked = table.Column<byte>(type: "tinyint(1) unsigned", nullable: false, defaultValue: (byte)0),
                    totalXp = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    levelRewarded = table.Column<byte>(type: "tinyint(4) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.path });
                    table.ForeignKey(
                        name: "FK__character_path_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_pet_customisation",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    type = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    objectId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    name = table.Column<string>(type: "varchar(128)", nullable: false, defaultValue: ""),
                    flairIdMask = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.type, x.objectId });
                    table.ForeignKey(
                        name: "FK__character_pet_customisation_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_pet_flair",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    petFlairId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.petFlairId });
                    table.ForeignKey(
                        name: "FK__character_pet_flair_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_quest",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    questId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    state = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    flags = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    timer = table.Column<uint>(type: "int(10) unsigned", nullable: true),
                    reset = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.questId });
                    table.ForeignKey(
                        name: "FK__character_quest_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_spell",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    spell4BaseId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    tier = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.spell4BaseId });
                    table.ForeignKey(
                        name: "FK__character_spell_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_stats",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    stat = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    value = table.Column<float>(type: "float", nullable: false, defaultValue: 0f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.stat });
                    table.ForeignKey(
                        name: "FK__character_stats_stat_id_character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_title",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    title = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    timeRemaining = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    revoked = table.Column<byte>(type: "tinyint(4) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.title });
                    table.ForeignKey(
                        name: "FK__character_title_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_zonemap_hexgroup",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    zoneMap = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    hexGroup = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.zoneMap, x.hexGroup });
                    table.ForeignKey(
                        name: "FK__character_zonemap_hexgroup_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "item",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    ownerId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: true),
                    itemId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    location = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    bagIndex = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    stackCount = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    charges = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    durability = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    expirationTimeLeft = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item", x => x.id);
                    table.ForeignKey(
                        name: "FK__item_ownerId__character_id",
                        column: x => x.ownerId,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "residence",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    ownerId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    propertyInfoId = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    name = table.Column<string>(type: "varchar(50)", nullable: false, defaultValue: ""),
                    privacyLevel = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    wallpaperId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    roofDecorInfoId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    entrywayDecorInfoId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    doorDecorInfoId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    groundWallpaperId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    musicId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    skyWallpaperId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    flags = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    resourceSharing = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    gardenSharing = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_residence", x => x.id);
                    table.ForeignKey(
                        name: "FK__residence_ownerId__character_id",
                        column: x => x.ownerId,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_costume_item",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    index = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    slot = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    itemId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    dyeData = table.Column<int>(type: "int(10)", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.index, x.slot });
                    table.ForeignKey(
                        name: "FK__character_costume_item_id-index__character_costume_id-index",
                        columns: x => new { x.id, x.index },
                        principalTable: "character_costume",
                        principalColumns: new[] { "id", "index" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_quest_objective",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    questId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    index = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    progress = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    timer = table.Column<uint>(type: "int(10) unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.questId, x.index });
                    table.ForeignKey(
                        name: "FK__character_quest_objective_id__character_id",
                        columns: x => new { x.id, x.questId },
                        principalTable: "character_quest",
                        principalColumns: new[] { "id", "questId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_mail_attachment",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    index = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    itemGuid = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.index });
                    table.ForeignKey(
                        name: "FK__character_mail_attachment_id__character_mail_id",
                        column: x => x.id,
                        principalTable: "character_mail",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__character_mail_attachment_itemGuid__item_id",
                        column: x => x.itemGuid,
                        principalTable: "item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "residence_decor",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    decorId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    decorInfoId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    decorType = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    plotIndex = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 2147483647u),
                    scale = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    x = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    y = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    z = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    qx = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    qy = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    qz = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    qw = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    decorParentId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    colourShiftId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.decorId });
                    table.ForeignKey(
                        name: "FK__residence_decor_id__residence_id",
                        column: x => x.id,
                        principalTable: "residence",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "residence_plot",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    index = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    plotInfoId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    plugItemId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    plugFacing = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    buildState = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.index });
                    table.ForeignKey(
                        name: "FK__residence_plot_id__residence_id",
                        column: x => x.id,
                        principalTable: "residence",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "accountId",
                table: "character",
                column: "accountId");

            migrationBuilder.CreateIndex(
                name: "FK__character_mail_recipientId__character_id",
                table: "character_mail",
                column: "recipientId");

            migrationBuilder.CreateIndex(
                name: "itemGuid",
                table: "character_mail_attachment",
                column: "itemGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK__item_ownerId__character_id",
                table: "item",
                column: "ownerId");

            migrationBuilder.CreateIndex(
                name: "ownerId",
                table: "residence",
                column: "ownerId",
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
