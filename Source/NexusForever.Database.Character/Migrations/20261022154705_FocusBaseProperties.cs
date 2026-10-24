using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusForever.Database.Character.Migrations
{
    /// <inheritdoc />
    public partial class FocusBaseProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 0.005 in combat / 0.02 out of combat is believed from gameplay videos.
            // DB__UnitProperty2.tbl carries 0.011 for both as its defaultValue.
            // TODO: verify recovery rates with packet captures
            migrationBuilder.UpdateData(
                table: "property_base",
                keyColumns: new[] { "property", "subtype", "type" },
                keyValues: new object[] { 107u, 0u, 0u },
                column: "value",
                value: 0.005f);

            // TODO: verify recovery rates with packet captures
            migrationBuilder.UpdateData(
                table: "property_base",
                keyColumns: new[] { "property", "subtype", "type" },
                keyValues: new object[] { 108u, 0u, 0u },
                column: "value",
                value: 0.02f);

            // Focus pool, property 5 BaseFocusPool, for the three Focus classes
            // (3 Esper, 4 Medic, 7 Spellslinger). 1000 is observed in gameplay videos; no shipped
            // table in build 16042 carries a player Focus pool.
            // TODO: verify recovery rates with packet captures
            migrationBuilder.InsertData(
                table: "property_base",
                columns: new[] { "property", "subtype", "type", "note", "value" },
                values: new object[,]
                {
                    { 5u, 3u, 1u, "Class - Esper - Base Focus Pool", 1000f },
                    { 5u, 4u, 1u, "Class - Medic - Base Focus Pool", 1000f },
                    { 5u, 7u, 1u, "Class - Spellslinger - Base Focus Pool", 1000f }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "property_base",
                keyColumns: new[] { "property", "subtype", "type" },
                keyValues: new object[] { 5u, 3u, 1u });

            migrationBuilder.DeleteData(
                table: "property_base",
                keyColumns: new[] { "property", "subtype", "type" },
                keyValues: new object[] { 5u, 4u, 1u });

            migrationBuilder.DeleteData(
                table: "property_base",
                keyColumns: new[] { "property", "subtype", "type" },
                keyValues: new object[] { 5u, 7u, 1u });

            // back to the pre-migration zeroes
            // TODO: verify recovery rates with packet captures
            migrationBuilder.UpdateData(
                table: "property_base",
                keyColumns: new[] { "property", "subtype", "type" },
                keyValues: new object[] { 107u, 0u, 0u },
                column: "value",
                value: 0f);

            migrationBuilder.UpdateData(
                table: "property_base",
                keyColumns: new[] { "property", "subtype", "type" },
                keyValues: new object[] { 108u, 0u, 0u },
                column: "value",
                value: 0f);
        }
    }
}
