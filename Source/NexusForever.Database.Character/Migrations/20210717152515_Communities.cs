using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    public partial class Communities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__residence_ownerId__character_id",
                table: "residence");

            migrationBuilder.AlterColumn<ulong>(
                name: "ownerId",
                table: "residence",
                type: "bigint(20) unsigned",
                nullable: true,
                oldClrType: typeof(ulong),
                oldType: "bigint(20) unsigned",
                oldDefaultValue: 0ul);

            migrationBuilder.AddColumn<ulong>(
                name: "guildOwnerId",
                table: "residence",
                type: "bigint(20) unsigned",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "communityPlotReservation",
                table: "guild_member",
                type: "int(11)",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.CreateIndex(
                name: "IX_residence_guildOwnerId",
                table: "residence",
                column: "guildOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK__residence_guildOwnerId__guild_id",
                table: "residence",
                column: "guildOwnerId",
                principalTable: "guild",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__residence_ownerId__character_id",
                table: "residence",
                column: "ownerId",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__residence_guildOwnerId__guild_id",
                table: "residence");

            migrationBuilder.DropForeignKey(
                name: "FK__residence_ownerId__character_id",
                table: "residence");

            migrationBuilder.DropIndex(
                name: "IX_residence_guildOwnerId",
                table: "residence");

            migrationBuilder.DropColumn(
                name: "guildOwnerId",
                table: "residence");

            migrationBuilder.DropColumn(
                name: "communityPlotReservation",
                table: "guild_member");

            migrationBuilder.AlterColumn<ulong>(
                name: "ownerId",
                table: "residence",
                type: "bigint(20) unsigned",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "bigint(20) unsigned",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK__residence_ownerId__character_id",
                table: "residence",
                column: "ownerId",
                principalTable: "character",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
