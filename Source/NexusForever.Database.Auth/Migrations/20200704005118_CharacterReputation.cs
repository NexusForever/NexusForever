using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Auth.Migrations
{
    public partial class CharacterReputation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 99u, "Command: ReputationUpdate" },
                    { 98u, "Category: Reputation" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 98u);

            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 99u);
        }
    }
}
