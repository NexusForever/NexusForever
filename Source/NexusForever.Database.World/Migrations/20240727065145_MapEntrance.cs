using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.World.Migrations
{
    /// <inheritdoc />
    public partial class MapEntrance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "map_entrance",
                columns: table => new
                {
                    mapId = table.Column<uint>(type: "int(10) unsigned", nullable: false),
                    team = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false),
                    worldLocationId = table.Column<uint>(type: "int(10) unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.mapId, x.team });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "map_entrance");
        }
    }
}
