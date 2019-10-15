using Microsoft.EntityFrameworkCore.Metadata;
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
                    Type = table.Column<byte>(nullable: false),
                    ObjectId = table.Column<uint>(nullable: false),
                    Note = table.Column<string>(type: "varchar(500)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disable", x => new { x.Type, x.ObjectId });
                });

            migrationBuilder.CreateTable(
                name: "entity",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Type = table.Column<byte>(nullable: false),
                    Creature = table.Column<uint>(nullable: false),
                    World = table.Column<ushort>(nullable: false),
                    Area = table.Column<ushort>(nullable: false),
                    X = table.Column<float>(nullable: false),
                    Y = table.Column<float>(nullable: false),
                    Z = table.Column<float>(nullable: false),
                    Rx = table.Column<float>(nullable: false),
                    Ry = table.Column<float>(nullable: false),
                    Rz = table.Column<float>(nullable: false),
                    DisplayInfo = table.Column<uint>(nullable: false),
                    OutfitInfo = table.Column<ushort>(nullable: false),
                    Faction1 = table.Column<ushort>(nullable: false),
                    Faction2 = table.Column<ushort>(nullable: false),
                    QuestChecklistIdx = table.Column<byte>(nullable: false, defaultValue: (byte)0),
                    ActivePropId = table.Column<ulong>(nullable: false, defaultValue: 0ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_category",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<uint>(nullable: false, defaultValue: 26u),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(150)", nullable: false),
                    Index = table.Column<uint>(nullable: false, defaultValue: 1u),
                    Visible = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_group",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DisplayFlags = table.Column<uint>(nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", nullable: false),
                    DisplayInfoOverride = table.Column<ushort>(nullable: false),
                    Visible = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_offer_group", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tutorial",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    Type = table.Column<uint>(nullable: false),
                    TriggerId = table.Column<uint>(nullable: false),
                    Note = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tutorial", x => new { x.Id, x.Type, x.TriggerId });
                });

            migrationBuilder.CreateTable(
                name: "entity_spline",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    SplineId = table.Column<ushort>(nullable: false),
                    Mode = table.Column<byte>(nullable: false),
                    Speed = table.Column<float>(nullable: false),
                    Fx = table.Column<float>(nullable: false),
                    Fy = table.Column<float>(nullable: false),
                    Fz = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_spline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entity_spline_entity_Id",
                        column: x => x.Id,
                        principalTable: "entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_stats",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    Stat = table.Column<byte>(nullable: false),
                    Value = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_stats", x => new { x.Id, x.Stat });
                    table.ForeignKey(
                        name: "FK_entity_stats_entity_Id",
                        column: x => x.Id,
                        principalTable: "entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    BuyPriceMultiplier = table.Column<float>(nullable: false, defaultValue: 1f),
                    SellPriceMultiplier = table.Column<float>(nullable: false, defaultValue: 1f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_vendor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entity_vendor_entity_Id",
                        column: x => x.Id,
                        principalTable: "entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor_category",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    Index = table.Column<uint>(nullable: false),
                    LocalisedTextId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_vendor_category", x => new { x.Id, x.Index });
                    table.ForeignKey(
                        name: "FK_entity_vendor_category_entity_Id",
                        column: x => x.Id,
                        principalTable: "entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entity_vendor_item",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    Index = table.Column<uint>(nullable: false),
                    CategoryIndex = table.Column<uint>(nullable: false),
                    ItemId = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entity_vendor_item", x => new { x.Id, x.Index });
                    table.ForeignKey(
                        name: "FK_entity_vendor_item_entity_Id",
                        column: x => x.Id,
                        principalTable: "entity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_group_category",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    CategoryId = table.Column<uint>(nullable: false),
                    Index = table.Column<byte>(nullable: false),
                    Visible = table.Column<byte>(nullable: false, defaultValue: (byte)1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_offer_group_category", x => new { x.Id, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_store_offer_group_category_store_category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "store_category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_store_offer_group_category_store_offer_group_Id",
                        column: x => x.Id,
                        principalTable: "store_offer_group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_item",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    GroupId = table.Column<uint>(nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", nullable: false),
                    DisplayFlags = table.Column<uint>(nullable: false),
                    Field6 = table.Column<long>(nullable: false),
                    Field7 = table.Column<byte>(nullable: false),
                    Visible = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_offer_item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_store_offer_item_store_offer_group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "store_offer_group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_item_data",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    ItemId = table.Column<ushort>(nullable: false),
                    Type = table.Column<uint>(nullable: false),
                    Amount = table.Column<uint>(nullable: false, defaultValue: 1u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_offer_item_data", x => new { x.Id, x.ItemId });
                    table.ForeignKey(
                        name: "FK_store_offer_item_data_store_offer_item_Id",
                        column: x => x.Id,
                        principalTable: "store_offer_item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "store_offer_item_price",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    CurrencyId = table.Column<byte>(nullable: false),
                    Price = table.Column<float>(nullable: false),
                    DiscountType = table.Column<byte>(nullable: false),
                    DiscountValue = table.Column<float>(nullable: false),
                    Field14 = table.Column<long>(nullable: false),
                    Expiry = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store_offer_item_price", x => new { x.Id, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_store_offer_item_price_store_offer_item_Id",
                        column: x => x.Id,
                        principalTable: "store_offer_item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_store_category_ParentId",
                table: "store_category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_store_offer_group_category_CategoryId",
                table: "store_offer_group_category",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_store_offer_item_GroupId",
                table: "store_offer_item",
                column: "GroupId");
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
