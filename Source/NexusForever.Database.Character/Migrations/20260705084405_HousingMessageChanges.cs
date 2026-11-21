using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Character.Migrations
{
    /// <inheritdoc />
    public partial class HousingMessageChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "propertyInfoId",
                table: "residence",
                type: "tinyint(3) unsigned",
                nullable: false,
                defaultValue: (byte)35,
                oldClrType: typeof(byte),
                oldType: "tinyint(3) unsigned",
                oldDefaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "propertyInfoId",
                table: "residence",
                type: "tinyint(3) unsigned",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint(3) unsigned",
                oldDefaultValue: (byte)35);
        }
    }
}
