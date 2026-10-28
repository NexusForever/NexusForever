using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Character.Migrations
{
    /// <inheritdoc />
    public partial class AmpSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "bonusAmpPower",
                table: "character",
                type: "smallint(6)",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "character_eldan_augmentation_unlock",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    eldanAugmentationId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.eldanAugmentationId });
                    table.ForeignKey(
                        name: "FK__character_eldan_augmentation_unlock_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_eldan_augmentation_unlock");

            migrationBuilder.DropColumn(
                name: "bonusAmpPower",
                table: "character");
        }
    }
}
