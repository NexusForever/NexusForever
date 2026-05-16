using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Query.Migrations
{
    /// <inheritdoc />
    public partial class HelloWorld : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character",
                columns: table => new
                {
                    characterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    realmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    name = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    realmName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    race = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    @class = table.Column<byte>(name: "class", type: "tinyint unsigned", nullable: false),
                    path = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    faction = table.Column<uint>(type: "int unsigned", nullable: false),
                    sex = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    currentRealmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    worldZoneId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    level = table.Column<uint>(type: "int unsigned", nullable: false),
                    guildName = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    lastOnline = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => new { x.characterId, x.realmId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_character_class",
                table: "character",
                column: "class");

            migrationBuilder.CreateIndex(
                name: "IX_character_currentRealmId",
                table: "character",
                column: "currentRealmId");

            migrationBuilder.CreateIndex(
                name: "IX_character_guildName",
                table: "character",
                column: "guildName");

            migrationBuilder.CreateIndex(
                name: "IX_character_level",
                table: "character",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "IX_character_name",
                table: "character",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_character_path",
                table: "character",
                column: "path");

            migrationBuilder.CreateIndex(
                name: "IX_character_race",
                table: "character",
                column: "race");

            migrationBuilder.CreateIndex(
                name: "IX_character_worldZoneId",
                table: "character",
                column: "worldZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character");
        }
    }
}
