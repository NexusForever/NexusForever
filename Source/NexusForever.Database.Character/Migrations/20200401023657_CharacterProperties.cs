using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    public partial class CharacterProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "property_base",
                columns: table => new
                {
                    type = table.Column<uint>(nullable: false, defaultValueSql: "'0'"),
                    subtype = table.Column<uint>(nullable: false, defaultValueSql: "'0'"),
                    property = table.Column<uint>(nullable: false, defaultValueSql: "'0'"),
                    modType = table.Column<ushort>(nullable: false, defaultValueSql: "'0'"),
                    value = table.Column<float>(nullable: false, defaultValueSql: "'0'"),
                    note = table.Column<string>(type: "varchar(100)", nullable: false, defaultValueSql: "''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.type, x.subtype, x.property });
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note" },
                values: new object[] { 0u, 0u, 0u, "Player - Base Strength" });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "modType", "note", "value" },
                values: new object[,]
                {
                    { 2u, 60u, 35u, (ushort)1, "Item - Implant - Attack Power per Eff. Level", 3.6f },
                    { 2u, 59u, 35u, (ushort)1, "Item - Augment - Attack Power per Eff. Level", 3.6f },
                    { 2u, 58u, 35u, (ushort)1, "Item - Support System - Attack Power per Eff. Level", 3.6f },
                    { 2u, 20u, 35u, (ushort)1, "Item - Weapon - Attack Power per Eff. Level", 83.53f },
                    { 2u, 7u, 35u, (ushort)1, "Item - Tool - Attack Power per Eff. Level", 3.6f },
                    { 2u, 6u, 35u, (ushort)1, "Item - Hands - Attack Power per Eff. Level", 2.7f },
                    { 2u, 1u, 36u, (ushort)1, "Item - Chest - Support Power per Eff. Level", 4.5f },
                    { 2u, 5u, 35u, (ushort)1, "Item - Feet - Attack Power per Eff. Level", 2.7f },
                    { 2u, 3u, 35u, (ushort)1, "Item - Head - Attack Power per Eff. Level", 2.7f },
                    { 2u, 2u, 35u, (ushort)1, "Item - Legs - Attack Power per Eff. Level", 4.5f },
                    { 2u, 1u, 35u, (ushort)1, "Item - Chest - Attack Power per Eff. Level", 4.5f },
                    { 2u, 6u, 7u, (ushort)1, "Item - Hands - HP per Eff. Level", 45f },
                    { 2u, 5u, 7u, (ushort)1, "Item - Feet - HP per Eff. Level", 45f },
                    { 2u, 4u, 7u, (ushort)1, "Item - Shoulder - HP per Eff. Level", 60f },
                    { 2u, 4u, 35u, (ushort)1, "Item - Shoulder - Attack Power per Eff. Level", 3.6f },
                    { 2u, 2u, 36u, (ushort)1, "Item - Legs - Support Power per Eff. Level", 4.5f },
                    { 2u, 3u, 36u, (ushort)1, "Item - Head - Support Power per Eff. Level", 2.7f },
                    { 2u, 4u, 36u, (ushort)1, "Item - Shoulder - Support Power per Eff. Level", 3.6f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[] { 2u, 43u, 175u, "Item - Shield - Base Shield Mitigation Percent", 0.625f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "modType", "note", "value" },
                values: new object[,]
                {
                    { 2u, 6u, 42u, (ushort)1, "Item - Hands - Armor per Eff. Level", 15f },
                    { 2u, 5u, 42u, (ushort)1, "Item - Feet - Armor per Eff. Level", 15f },
                    { 2u, 4u, 42u, (ushort)1, "Item - Shoulder - Armor per Eff. Level", 20f },
                    { 2u, 3u, 42u, (ushort)1, "Item - Head - Armor per Eff. Level", 15f },
                    { 2u, 2u, 42u, (ushort)1, "Item - Legs - Armor per Eff. Level", 25f },
                    { 2u, 1u, 42u, (ushort)1, "Item - Chest - Armor per Eff. Level", 25f },
                    { 2u, 43u, 41u, (ushort)1, "Item - Shield - Shield Capacity per Eff. Level", 225f },
                    { 2u, 60u, 36u, (ushort)1, "Item - Implant - Support Power per Eff. Level", 3.6f },
                    { 2u, 59u, 36u, (ushort)1, "Item - Augment - Support Power per Eff. Level", 3.6f },
                    { 2u, 58u, 36u, (ushort)1, "Item - Support System - Support Power per Eff. Level", 3.6f },
                    { 2u, 20u, 36u, (ushort)1, "Item - Weapon - Support Power per Eff. Level", 83.53f },
                    { 2u, 7u, 36u, (ushort)1, "Item - Tool - Support Power per Eff. Level", 3.6f },
                    { 2u, 6u, 36u, (ushort)1, "Item - Hands - Support Power per Eff. Level", 2.7f },
                    { 2u, 5u, 36u, (ushort)1, "Item - Feet - Support Power per Eff. Level", 2.7f },
                    { 2u, 3u, 7u, (ushort)1, "Item - Head - HP per Eff. Level", 45f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[] { 2u, 43u, 176u, "Item - Shield - Base Shield Regen Percent", 0.15f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "modType", "note", "value" },
                values: new object[,]
                {
                    { 2u, 2u, 7u, (ushort)1, "Item - Legs - HP per Eff. Level", 75f },
                    { 1u, 7u, 13u, (ushort)2, "Class - Spellslinger - Base Spell Power Cap", 100f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[,]
                {
                    { 0u, 0u, 101u, "Player - Base Avoid Chance", 0.05f },
                    { 0u, 0u, 100u, "Player - Base Movement Speed", 1f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note" },
                values: new object[] { 0u, 0u, 41u, "Player - Shield Capacity Base" });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[,]
                {
                    { 0u, 0u, 39u, "Player - Base Dash Energy Regen", 0.045f },
                    { 0u, 0u, 38u, "Player - Base Dash Energy", 200f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "modType", "note", "value" },
                values: new object[] { 0u, 0u, 36u, (ushort)1, "Player - Base Support Rating per Level", 18f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[] { 0u, 0u, 102u, "Player - Base Crit Chance", 0.05f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "modType", "note", "value" },
                values: new object[] { 0u, 0u, 35u, (ushort)1, "Player - Base Assault Rating per Level", 18f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[] { 0u, 0u, 9u, "Player - Base Endurance", 500f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "modType", "note", "value" },
                values: new object[] { 0u, 0u, 7u, (ushort)1, "Player - Base HP per Level", 200f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note" },
                values: new object[,]
                {
                    { 0u, 0u, 4u, "Player - Base Wisdom" },
                    { 0u, 0u, 3u, "Player - Base Magic" },
                    { 0u, 0u, 2u, "Player - Base Technology" },
                    { 0u, 0u, 1u, "Player - Base Dexterity" }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[] { 0u, 0u, 16u, "Player - Base Endurance Regen", 0.0225f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note" },
                values: new object[,]
                {
                    { 0u, 0u, 107u, "Player - Base Focus Recovery In Combat" },
                    { 0u, 0u, 108u, "Player - Base Focus Recovery Out of Combat" }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[] { 0u, 0u, 112u, "Player - Base Multi-Hit Amount", 0.3f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "modType", "note", "value" },
                values: new object[,]
                {
                    { 1u, 5u, 19u, (ushort)2, "Class - Stalker - Base Suit Power Regeneration Rate", 0.035f },
                    { 1u, 5u, 12u, (ushort)2, "Class - Stalker - Base Suit Power Cap", 100f },
                    { 1u, 4u, 10u, (ushort)2, "Class - Medic - Base Medic Core Cap", 4f },
                    { 1u, 3u, 10u, (ushort)2, "Class - Esper - Base Psi Point Cap", 5f },
                    { 1u, 2u, 10u, (ushort)2, "Class - Engineer - Base Volatile Energy Cap", 100f },
                    { 0u, 0u, 17u, (ushort)2, "Warrior - Base Kinetic Energy Regen", 1f },
                    { 1u, 1u, 10u, (ushort)2, "Class - Warrior - Base Kinetic Energy Cap", 1000f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[,]
                {
                    { 0u, 0u, 195u, "Player - Base Glance Amount", 0.3f },
                    { 0u, 0u, 191u, "Player - Base Mount Movement Speed", 1f },
                    { 0u, 0u, 155u, "Player - Base Damage Reflect Amount", 0.05f },
                    { 0u, 0u, 154u, "Player - Base Mutli-Hit Chance", 0.05f },
                    { 0u, 0u, 152u, "Player - Base Damage Taken Offset - Magic", 1f },
                    { 0u, 0u, 151u, "Player - Base Damage Taken Offset - Tech", 1f },
                    { 0u, 0u, 150u, "Player - Base Damage Taken Offset - Physical", 1f },
                    { 0u, 0u, 130u, "Player - Base Gravity Multiplier", 0.8f }
                });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "modType", "note", "value" },
                values: new object[] { 2u, 1u, 7u, (ushort)1, "Item - Chest - HP per Eff. Level", 75f });

            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "type", "subtype", "property", "note", "value" },
                values: new object[] { 2u, 43u, 178u, "Item - Shield - Base Shield Reboot Rate (ms)", 5130f });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "property_base");
        }
    }
}
