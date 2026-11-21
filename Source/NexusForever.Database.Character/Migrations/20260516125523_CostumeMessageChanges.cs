using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Character.Migrations
{
    /// <inheritdoc />
    public partial class CostumeMessageChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "itemId",
                table: "character_costume_item",
                newName: "item2Id");

            migrationBuilder.RenameColumn(
                name: "mask",
                table: "character_costume",
                newName: "visibilityMask");

            migrationBuilder.AlterColumn<uint>(
                name: "dyeData",
                table: "character_costume_item",
                type: "int(10) unsigned",
                nullable: false,
                defaultValue: 0u,
                oldClrType: typeof(int),
                oldType: "int(10)",
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "item2Id",
                table: "character_costume_item",
                newName: "itemId");

            migrationBuilder.RenameColumn(
                name: "visibilityMask",
                table: "character_costume",
                newName: "mask");

            migrationBuilder.AlterColumn<int>(
                name: "dyeData",
                table: "character_costume_item",
                type: "int(10)",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(uint),
                oldType: "int(10) unsigned",
                oldDefaultValue: 0u);
        }
    }
}
