using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.World.Migrations
{
    [DbContext(typeof(WorldContext))]
    [Migration("20261127020000_SeasonalEvents")]
    public sealed class SeasonalEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seasonal_event",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false),
                    startMonth = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false),
                    startDay = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false),
                    startHourUtc = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false),
                    durationHours = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false)
                },
                constraints: table => table.PrimaryKey("PRIMARY", row => row.id))
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "seasonal_event_quest",
                columns: table => new
                {
                    eventId = table.Column<uint>(type: "int(10) unsigned", nullable: false),
                    questId = table.Column<uint>(type: "int(10) unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", row => new { row.eventId, row.questId });
                    table.ForeignKey(
                        name: "FK__seasonal_event_quest_eventId__seasonal_event_id",
                        column: row => row.eventId,
                        principalTable: "seasonal_event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "seasonal_event_entity",
                columns: table => new
                {
                    eventId = table.Column<uint>(type: "int(10) unsigned", nullable: false),
                    entityId = table.Column<uint>(type: "int(10) unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", row => new { row.eventId, row.entityId });
                    table.ForeignKey(
                        name: "FK__seasonal_event_entity_eventId__seasonal_event_id",
                        column: row => row.eventId,
                        principalTable: "seasonal_event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__seasonal_event_entity_entityId__entity_id",
                        column: row => row.entityId,
                        principalTable: "entity",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_seasonal_event_entity_entityId",
                table: "seasonal_event_entity",
                column: "entityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("seasonal_event_entity");
            migrationBuilder.DropTable("seasonal_event_quest");
            migrationBuilder.DropTable("seasonal_event");
        }
    }
}
