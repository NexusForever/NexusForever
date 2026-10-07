using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Character.Migrations
{
    /// <inheritdoc />
    public partial class CharacterReturnPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ushort>(
                name: "returnWorldId",
                table: "character",
                type: "smallint(5) unsigned",
                nullable: false,
                defaultValue: (ushort)0);

            migrationBuilder.AddColumn<float>(
                name: "returnLocationX",
                table: "character",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "returnLocationY",
                table: "character",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "returnLocationZ",
                table: "character",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "returnWorldId",
                table: "character");

            migrationBuilder.DropColumn(
                name: "returnLocationX",
                table: "character");

            migrationBuilder.DropColumn(
                name: "returnLocationY",
                table: "character");

            migrationBuilder.DropColumn(
                name: "returnLocationZ",
                table: "character");
        }
    }
}
