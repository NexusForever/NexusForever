using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    public partial class CharacterRewardTrack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_reward_track",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    rewardTrackId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    points = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.rewardTrackId });
                    table.ForeignKey(
                        name: "FK__character_reward_track_id__character_id",
                        column: x => x.id,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "character_reward_track_milestone",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    rewardTrackId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    milestoneId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    pointsRequired = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    choice = table.Column<int>(nullable: false, defaultValue: -1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.rewardTrackId, x.milestoneId });
                    table.ForeignKey(
                        name: "FK__character_reward_track_milestone_id-rewardTrackId__character_reward_track_id-rewardTrackId",
                        columns: x => new { x.id, x.rewardTrackId },
                        principalTable: "character_reward_track",
                        principalColumns: new[] { "id", "rewardTrackId" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_reward_track_milestone");

            migrationBuilder.DropTable(
                name: "character_reward_track");
        }
    }
}
