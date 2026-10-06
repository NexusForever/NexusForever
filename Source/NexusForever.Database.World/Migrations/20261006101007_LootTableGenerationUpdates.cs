using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.World.Migrations
{
    /// <inheritdoc />
    public partial class LootTableGenerationUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "minDrop",
                table: "loot_group",
                newName: "minCount");

            migrationBuilder.RenameColumn(
                name: "maxDrop",
                table: "loot_group",
                newName: "maxCount");

            migrationBuilder.AlterColumn<byte>(
                name: "type",
                table: "loot_item",
                type: "tinyint(3) unsigned",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(uint),
                oldType: "int(10) unsigned",
                oldDefaultValue: 0u);

            migrationBuilder.AlterColumn<byte>(
                name: "conditionType",
                table: "loot_group",
                type: "tinyint(3) unsigned",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(uint),
                oldType: "int(10) unsigned",
                oldDefaultValue: 0u);

            migrationBuilder.AlterColumn<ulong>(
                name: "lootGroupId",
                table: "item_loot",
                type: "bigint(20) unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bigint(20) unsigned");

            migrationBuilder.AlterColumn<ulong>(
                name: "lootGroupId",
                table: "entity_loot",
                type: "bigint(20) unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bigint(20) unsigned");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "minCount",
                table: "loot_group",
                newName: "minDrop");

            migrationBuilder.RenameColumn(
                name: "maxCount",
                table: "loot_group",
                newName: "maxDrop");

            migrationBuilder.AlterColumn<uint>(
                name: "type",
                table: "loot_item",
                type: "int(10) unsigned",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(byte),
                oldType: "tinyint(3) unsigned",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<uint>(
                name: "conditionType",
                table: "loot_group",
                type: "int(10) unsigned",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(byte),
                oldType: "tinyint(3) unsigned",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<ulong>(
                name: "lootGroupId",
                table: "item_loot",
                type: "bigint(20) unsigned",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint(20) unsigned",
                oldDefaultValue: 0ul);

            migrationBuilder.AlterColumn<ulong>(
                name: "lootGroupId",
                table: "entity_loot",
                type: "bigint(20) unsigned",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint(20) unsigned",
                oldDefaultValue: 0ul);
        }
    }
}
