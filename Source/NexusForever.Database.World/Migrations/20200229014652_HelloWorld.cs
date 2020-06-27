using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.World.Migrations
{
    public partial class HelloWorld : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "disable",
                columns: table => new
                {
                    type = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    objectId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    note = table.Column<string>(type: "varchar(500)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.type, x.objectId });
                });

            migrationBuilder.CreateTable(
                name: "entity",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    type = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    creature = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    world = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    area = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    x = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    y = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    z = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    rx = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    ry = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    rz = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    displayInfo = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    outfitInfo = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    faction1 = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    faction2 = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    questChecklistIdx = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    activePropId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "store_category",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    parentId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 26u),
                    name = table.Column<string>(type: "varchar(50)", nullable: false, defaultValue: ""),
                    description = table.Column<string>(type: "varchar(150)", nullable: false, defaultValue: ""),
                    index = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 1u),
                    visible = table.Column<byte>(type: "tinyint(1) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_group",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    displayFlags = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    name = table.Column<string>(type: "varchar(50)", nullable: false, defaultValue: ""),
                    description = table.Column<string>(type: "varchar(500)", nullable: false, defaultValue: ""),
                    displayInfoOverride = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    visible = table.Column<byte>(type: "tinyint(1) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_offer_group", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tutorial",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u, comment: "Tutorial ID"),
                    type = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    triggerId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    note = table.Column<string>(type: "varchar(50)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.type, x.triggerId });
                });

            migrationBuilder.CreateTable(
                name: "entity_spline",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    splineId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    mode = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    speed = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    fx = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    fy = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    fz = table.Column<float>(type: "float", nullable: false, defaultValue: 0f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_spline", x => x.id);
                    table.ForeignKey(
                        name: "FK__entity_spline_id__entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_stats",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    stat = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    value = table.Column<float>(type: "float", nullable: false, defaultValue: 0f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.stat });
                    table.ForeignKey(
                        name: "FK__entity_stats_stat_id_entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    buyPriceMultiplier = table.Column<float>(type: "float", nullable: false, defaultValue: 1f),
                    sellPriceMultiplier = table.Column<float>(type: "float", nullable: false, defaultValue: 1f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_vendor", x => x.id);
                    table.ForeignKey(
                        name: "FK__entity_vendor_id__entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor_category",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    index = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    localisedTextId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.index });
                    table.ForeignKey(
                        name: "FK__entity_vendor_category_id__entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor_item",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    index = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    categoryIndex = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    itemId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.index });
                    table.ForeignKey(
                        name: "FK__entity_vendor_item_id__entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_group_category",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    categoryId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    index = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    visible = table.Column<byte>(type: "tinyint(1) unsigned", nullable: false, defaultValue: (byte)1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.categoryId });
                    table.ForeignKey(
                        name: "FK__store_offer_group_category_categoryId__store_category_id",
                        column: x => x.categoryId,
                        principalTable: "store_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__store_offer_group_category_id__store_offer_group_id",
                        column: x => x.id,
                        principalTable: "store_offer_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_item",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    groupId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    name = table.Column<string>(type: "varchar(50)", nullable: false, defaultValue: ""),
                    description = table.Column<string>(type: "varchar(500)", nullable: false, defaultValue: ""),
                    displayFlags = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    field_6 = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 0L),
                    field_7 = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    visible = table.Column<byte>(type: "tinyint(1) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.groupId });
                    table.UniqueConstraint("AK_store_offer_item_id", x => x.id);
                    table.ForeignKey(
                        name: "FK__store_offer_item_groupId__store_offer_group_id",
                        column: x => x.groupId,
                        principalTable: "store_offer_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_item_data",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    itemId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    type = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    amount = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 1u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.itemId });
                    table.ForeignKey(
                        name: "FK__store_offer_item_data_id__store_offer_item_id",
                        column: x => x.id,
                        principalTable: "store_offer_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_item_price",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    currencyId = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    price = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    discountType = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    discountValue = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    field_14 = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 0L),
                    expiry = table.Column<long>(type: "bigint(20)", nullable: false, defaultValue: 0L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.currencyId });
                    table.ForeignKey(
                        name: "FK__store_offer_item_price_id__store_offer_item_id",
                        column: x => x.id,
                        principalTable: "store_offer_item",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "parentId",
                table: "store_category",
                column: "parentId");

            migrationBuilder.CreateIndex(
                name: "FK__store_offer_group_category_categoryId__store_category_id",
                table: "store_offer_group_category",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "FK__store_offer_item_groupId__store_offer_group_id",
                table: "store_offer_item",
                column: "groupId");

            migrationBuilder.CreateIndex(
                name: "id",
                table: "store_offer_item",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "disable");

            migrationBuilder.DropTable(
                name: "entity_spline");

            migrationBuilder.DropTable(
                name: "entity_stats");

            migrationBuilder.DropTable(
                name: "entity_vendor");

            migrationBuilder.DropTable(
                name: "entity_vendor_category");

            migrationBuilder.DropTable(
                name: "entity_vendor_item");

            migrationBuilder.DropTable(
                name: "store_offer_group_category");

            migrationBuilder.DropTable(
                name: "store_offer_item_data");

            migrationBuilder.DropTable(
                name: "store_offer_item_price");

            migrationBuilder.DropTable(
                name: "tutorial");

            migrationBuilder.DropTable(
                name: "entity");

            migrationBuilder.DropTable(
                name: "store_category");

            migrationBuilder.DropTable(
                name: "store_offer_item");

            migrationBuilder.DropTable(
                name: "store_offer_group");
        }
    }
}
