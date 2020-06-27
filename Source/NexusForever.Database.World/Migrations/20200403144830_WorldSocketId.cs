using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.World.Migrations
{
    public partial class WorldSocketId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ushort>(
                name: "worldSocketId",
                table: "entity",
                type: "smallint(5) unsigned",
                nullable: false,
                defaultValue: (ushort)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "worldSocketId",
                table: "entity");
        }
    }
}
