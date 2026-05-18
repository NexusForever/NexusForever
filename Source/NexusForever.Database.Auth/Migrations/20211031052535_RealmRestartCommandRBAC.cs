using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Auth.Migrations
{
    public partial class RealmRestartCommandRBAC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[] { 107u, "Category: RealmShutdown" });

            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[] { 108u, "Command: RealmShutdownStart" });

            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[] { 109u, "Command: RealmShutdownCancel" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 107u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 108u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 109u);
        }
    }
}
