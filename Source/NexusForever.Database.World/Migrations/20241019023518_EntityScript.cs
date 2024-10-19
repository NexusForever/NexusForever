using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.World.Migrations
{
    /// <inheritdoc />
    public partial class EntityScript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entity_script",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    scriptName = table.Column<string>(type: "varchar(150)", nullable: false, defaultValue: "")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.scriptName });
                    table.ForeignKey(
                        name: "FK__entity_script_id__entity_id",
                        column: x => x.id,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entity_script");
        }
    }
}
