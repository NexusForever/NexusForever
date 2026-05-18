using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusForever.Database.Character.Migrations
{
    /// <inheritdoc />
    public partial class BaseProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "property_base",
                columns: table => new
                {
                    type = table.Column<uint>(type: "int unsigned", nullable: false, defaultValueSql: "'0'"),
                    subtype = table.Column<uint>(type: "int unsigned", nullable: false, defaultValueSql: "'0'"),
                    property = table.Column<uint>(type: "int unsigned", nullable: false, defaultValueSql: "'0'"),
                    modType = table.Column<ushort>(type: "smallint unsigned", nullable: false, defaultValueSql: "'0'"),
                    value = table.Column<float>(type: "float", nullable: false, defaultValueSql: "'0'"),
                    note = table.Column<string>(type: "varchar(100)", nullable: false, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.type, x.subtype, x.property });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "note" },
                values: new object[,]
                {
                    { 0u, 0u, 0u, "Player - Base Strength" },
                    { 1u, 0u, 0u, "Player - Base Dexterity" },
                    { 2u, 0u, 0u, "Player - Base Technology" },
                    { 3u, 0u, 0u, "Player - Base Magic" },
                    { 4u, 0u, 0u, "Player - Base Wisdom" }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "modType", "note", "value" },
                values: new object[] { 7u, 0u, 0u, (ushort)3, "Player - Base HP per Level", 200f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "note", "value" },
                values: new object[,]
                {
                    { 9u, 0u, 0u, "Player - Base Endurance", 500f },
                    { 16u, 0u, 0u, "Player - Base Endurance Regen", 0.0225f },
                    { 17u, 0u, 0u, "Warrior - Base Kinetic Energy Regen", 1f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "modType", "note", "value" },
                values: new object[,]
                {
                    { 35u, 0u, 0u, (ushort)3, "Player - Base Assault Rating per Level", 18f },
                    { 36u, 0u, 0u, (ushort)3, "Player - Base Support Rating per Level", 18f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "note", "value" },
                values: new object[,]
                {
                    { 38u, 0u, 0u, "Player - Base Dash Energy", 200f },
                    { 39u, 0u, 0u, "Player - Base Dash Energy Regen", 0.045f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "note" },
                values: new object[] { 41u, 0u, 0u, "Player - Shield Capacity Base" });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "note", "value" },
                values: new object[,]
                {
                    { 100u, 0u, 0u, "Player - Base Movement Speed", 1f },
                    { 101u, 0u, 0u, "Player - Base Avoid Chance", 0.05f },
                    { 102u, 0u, 0u, "Player - Base Crit Chance", 0.05f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "note" },
                values: new object[,]
                {
                    { 107u, 0u, 0u, "Player - Base Focus Recovery In Combat" },
                    { 108u, 0u, 0u, "Player - Base Focus Recovery Out of Combat" }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "note", "value" },
                values: new object[,]
                {
                    { 112u, 0u, 0u, "Player - Base Multi-Hit Amount", 0.3f },
                    { 130u, 0u, 0u, "Player - Base Gravity Multiplier", 0.8f },
                    { 150u, 0u, 0u, "Player - Base Damage Taken Offset - Physical", 1f },
                    { 151u, 0u, 0u, "Player - Base Damage Taken Offset - Tech", 1f },
                    { 152u, 0u, 0u, "Player - Base Damage Taken Offset - Magic", 1f },
                    { 154u, 0u, 0u, "Player - Base Mutli-Hit Chance", 0.05f },
                    { 155u, 0u, 0u, "Player - Base Damage Reflect Amount", 0.05f },
                    { 191u, 0u, 0u, "Player - Base Mount Movement Speed", 1f },
                    { 195u, 0u, 0u, "Player - Base Glance Amount", 0.3f },
                    { 10u, 1u, 1u, "Class - Warrior - Base Kinetic Energy Cap", 1000f },
                    { 10u, 2u, 1u, "Class - Engineer - Base Volatile Energy Cap", 100f },
                    { 10u, 3u, 1u, "Class - Esper - Base Psi Point Cap", 5f },
                    { 10u, 4u, 1u, "Class - Medic - Base Medic Core Cap", 4f },
                    { 12u, 5u, 1u, "Class - Stalker - Base Suit Power Cap", 100f },
                    { 19u, 5u, 1u, "Class - Stalker - Base Suit Power Regeneration Rate", 0.035f },
                    { 13u, 7u, 1u, "Class - Spellslinger - Base Spell Power Cap", 100f }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "property_base");
        }
    }
}
