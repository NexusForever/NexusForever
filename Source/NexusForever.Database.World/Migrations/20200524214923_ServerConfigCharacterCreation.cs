using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.World.Migrations
{
    public partial class ServerConfigCharacterCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "server_config",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    active = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_config", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "server_config",
                columns: new[] { "id", "active" },
                values: new object[,]
                {
                    { 1u, 1u }
                });

            migrationBuilder.CreateTable(
                name: "server_config_character_creation_location",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    characterCreationId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    raceId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    factionId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    locationX = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    locationY = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    locationZ = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    rotationX = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    worldId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    worldZoneId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.characterCreationId, x.raceId, x.factionId });
                    table.ForeignKey(
                        name: "FK__server_config_config_id_creation_id",
                        column: x => x.id,
                        principalTable: "server_config",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.InsertData(
                table: "server_config_character_creation_location",
                columns: new[] { "id", "characterCreationId", "raceId", "factionId", "locationX", "locationY", "locationZ", "rotationX","worldId", "worldZoneId" },
                values: new object[,]
                {
                    //Nexus
                    { 1u, 3u,  1u, 166u,  -3780.44f, -996.21f,  -6342.73f, -2.80f,   22u, 1410u },
                    { 1u, 3u, 12u, 166u,  -3780.44f, -996.21f,  -6342.73f, -2.80f,   22u, 1410u },
                    { 1u, 3u,  5u, 166u, -23069.51f, -994.85f, -27376.68f, -2.35f, 2997u, 1956u },
                    { 1u, 3u, 13u, 166u, -23069.51f, -994.85f, -27376.68f, -2.35f, 2997u, 1956u },
                    { 1u, 3u,  1u, 167u,   4078.21f, -657.08f,  -5138.12f, -0.27f,  426u,  648u },
                    { 1u, 3u,  3u, 167u,   4078.21f, -657.08f,  -5138.12f, -0.27f,  426u,  648u },
                    { 1u, 3u,  4u, 167u,   -771.36f, -904.01f,  -2267.78f, -0.99f,  990u, 1417u },
                    { 1u, 3u, 16u, 167u,   -771.36f, -904.01f,  -2267.78f, -0.99f,  990u, 1417u },
                    //Tutorial
                    { 1u, 4u,  1u, 166u,     29.13f, -853.87f,   -560.19f, -2.91f,   3460u, 5966u },
                    { 1u, 4u, 12u, 166u,     29.13f, -853.87f,   -560.19f, -2.91f,   3460u, 5966u },
                    { 1u, 4u,  5u, 166u,     29.13f, -853.87f,   -560.19f, -2.91f,   3460u, 5966u },
                    { 1u, 4u, 13u, 166u,     29.13f, -853.87f,   -560.19f, -2.91f,   3460u, 5966u },
                    { 1u, 4u,  1u, 167u,     29.13f, -853.87f,   -560.19f, -2.91f,   3460u, 5966u },
                    { 1u, 4u,  3u, 167u,     29.13f, -853.87f,   -560.19f, -2.91f,   3460u, 5966u },
                    { 1u, 4u,  4u, 167u,     29.13f, -853.87f,   -560.19f, -2.91f,   3460u, 5966u },
                    { 1u, 4u, 16u, 167u,     29.13f, -853.87f,   -560.19f, -2.91f,   3460u, 5966u },
                });
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "server_config_character_creation_location");

            migrationBuilder.DropTable(
                name: "server_config");
        }
    }
}
