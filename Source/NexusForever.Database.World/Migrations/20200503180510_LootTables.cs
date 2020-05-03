using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.World.Migrations
{
    public partial class LootTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "loot_group",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    parentId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: true),
                    probability = table.Column<float>(type: "float", nullable: false, defaultValue: 100f),
                    minDrop = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    maxDrop = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    condition = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    comment = table.Column<string>(type: "varchar(200)", nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loot_group", x => x.id);
                    table.ForeignKey(
                        name: "FK__loot_group_parentId__loot_group_id",
                        column: x => x.parentId,
                        principalTable: "loot_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "item_loot",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    lootGroupId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: true),
                    comment = table.Column<string>(type: "varchar(200)", nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_loot_loot_group_lootGroupId",
                        column: x => x.lootGroupId,
                        principalTable: "loot_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "loot_item",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    type = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    staticId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    probability = table.Column<float>(type: "float", nullable: false, defaultValue: 100f),
                    minCount = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    maxCount = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    comment = table.Column<string>(type: "varchar(200)", nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.type, x.staticId });
                    table.ForeignKey(
                        name: "FK__loot_item_id__loot_group_id",
                        column: x => x.id,
                        principalTable: "loot_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_item_loot_lootGroupId",
                table: "item_loot",
                column: "lootGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_loot_group_parentId",
                table: "loot_group",
                column: "parentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_loot");

            migrationBuilder.DropTable(
                name: "loot_item");

            migrationBuilder.DropTable(
                name: "loot_group");
        }
    }
}
