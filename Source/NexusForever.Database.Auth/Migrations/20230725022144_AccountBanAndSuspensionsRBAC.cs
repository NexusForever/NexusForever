using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusForever.Database.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AccountBanAndSuspensionsRBAC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 117u, "Category: Ban" },
                    { 118u, "Category: BanAccount" },
                    { 119u, "Command: BanAccountPlayer" },
                    { 120u, "Command: BanAccountCharacter" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 117u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 118u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 119u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 120u);
        }
    }
}
