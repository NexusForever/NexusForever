using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    [DbContext(typeof(CharacterContext))]
    [Migration("20261124012000_QuestObjectiveChecklist")]
    public sealed class QuestObjectiveChecklist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "checklist",
                table: "character_quest_objective",
                type: "int(10) unsigned",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "checklist", table: "character_quest_objective");
        }
    }
}
