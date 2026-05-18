using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Auth.Migrations
{
    public partial class EntitlementRBAC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 38u);

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 41u,
                column: "name",
                value: "Command: EntitlementAdd");

            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[] { 10004u, "Other: EntitlementGrantOther" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 10004u);

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 41u,
                column: "name",
                value: "Command: EntitlementAccountAdd");

            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[] { 38u, "Command: EntitlementCharacterAdd" });
        }
    }
}
