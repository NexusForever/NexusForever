using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.World.Migrations
{
    /// <inheritdoc />
    public partial class EntityProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entity_property",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    property = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false),
                    value = table.Column<float>(type: "float", nullable: false, defaultValue: 0f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.property });
                    table.ForeignKey(
                        name: "FK__entity_property_id__entity_id",
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
                name: "entity_property");
        }
    }
}
