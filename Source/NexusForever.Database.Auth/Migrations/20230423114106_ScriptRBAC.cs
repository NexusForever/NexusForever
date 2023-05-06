using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusForever.Database.Auth.Migrations
{
    /// <inheritdoc />
    public partial class ScriptRBAC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 112u, "Category: Script" },
                    { 113u, "Command: ScriptReload" },
                    { 114u, "Command: ScriptInfo" },
                    { 115u, "Command: ScriptAdd" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 112u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 113u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 114u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 115u);
        }
    }
}
