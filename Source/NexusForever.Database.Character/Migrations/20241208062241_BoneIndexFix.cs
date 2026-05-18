using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Character.Migrations
{
    /// <inheritdoc />
    public partial class BoneIndexFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "boneIndex",
                table: "character_bone",
                type: "tinyint(4) unsigned",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint(4) unsigned",
                oldDefaultValue: (byte)0);

            migrationBuilder.AlterColumn<ulong>(
                name: "id",
                table: "character_bone",
                type: "bigint(20) unsigned",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint(20) unsigned",
                oldDefaultValue: 0ul);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "boneIndex",
                table: "character_bone",
                type: "tinyint(4) unsigned",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint(4) unsigned");

            migrationBuilder.AlterColumn<ulong>(
                name: "id",
                table: "character_bone",
                type: "bigint(20) unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bigint(20) unsigned");
        }
    }
}
