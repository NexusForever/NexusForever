using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    public partial class StartingLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_create",
                columns: table => new
                {
                    race = table.Column<byte>(type: "tinyint(4) unsigned", nullable: false, defaultValue: (byte)0),
                    creationStart = table.Column<byte>(type: "tinyint(4) unsigned", nullable: false, defaultValue: (byte)0),
                    faction = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    worldId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    x = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    y = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    z = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    rx = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    ry = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    rz = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    comment = table.Column<string>(type: "varchar(200)", nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.race, x.faction, x.creationStart });
                });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "rx", "worldId", "x", "y", "z" },
                values: new object[,]
                {
                    { (byte)4, (ushort)167, (byte)1, "Exile Human - Novice", -2.751458f, 3460u, 29.1286f, -853.8716f, -560.188f },
                    { (byte)4, (ushort)166, (byte)12, "Dominion Mechari - Novice", -2.751458f, 3460u, 29.1286f, -853.8716f, -560.188f },
                    { (byte)5, (ushort)166, (byte)1, "Dominion Cassian - Level 50", -0.7632219f, 22u, -3343.58f, -887.4646f, -536.03f },
                    { (byte)3, (ushort)166, (byte)1, "Dominion Cassian - Veteran", -0.45682f, 1387u, -3835.341f, -980.2174f, -6050.524f },
                    { (byte)4, (ushort)166, (byte)1, "Dominion Cassian - Novice", -2.751458f, 3460u, 29.1286f, -853.8716f, -560.188f },
                    { (byte)5, (ushort)166, (byte)5, "Dominion Draken - Level 50", -0.7632219f, 22u, -3343.58f, -887.4646f, -536.03f },
                    { (byte)3, (ushort)166, (byte)5, "Dominion Draken - Veteran", -2.215535f, 870u, -8261.398f, -995.471f, -242.3648f },
                    { (byte)4, (ushort)166, (byte)5, "Dominion Draken - Novice", -2.751458f, 3460u, 29.1286f, -853.8716f, -560.188f },
                    { (byte)5, (ushort)166, (byte)13, "Dominion Chua - Level 50", -0.7632219f, 22u, -3343.58f, -887.4646f, -536.03f },
                    { (byte)3, (ushort)166, (byte)13, "Dominion Chua - Veteran", -2.215535f, 870u, -8261.398f, -995.471f, -242.3648f },
                    { (byte)4, (ushort)166, (byte)13, "Dominion Chua - Novice", -2.751458f, 3460u, 29.1286f, -853.8716f, -560.188f }
                });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "worldId", "x", "y", "z" },
                values: new object[] { (byte)5, (ushort)167, (byte)16, "Exile Mordesh - Level 50", 51u, 4074.34f, -797.8368f, -2399.37f });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "rx", "worldId", "x", "y", "z" },
                values: new object[,]
                {
                    { (byte)3, (ushort)167, (byte)16, "Exile Mordesh - Veteran", -1.1214035f, 990u, -771.823f, -904.2852f, -2269.56f },
                    { (byte)4, (ushort)167, (byte)16, "Exile Mordesh - Novice", -2.751458f, 3460u, 29.1286f, -853.8716f, -560.188f }
                });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "worldId", "x", "y", "z" },
                values: new object[] { (byte)5, (ushort)167, (byte)4, "Exile Aurin - Level 50", 51u, 4074.34f, -797.8368f, -2399.37f });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "rx", "worldId", "x", "y", "z" },
                values: new object[,]
                {
                    { (byte)3, (ushort)167, (byte)4, "Exile Aurin - Veteran", -1.1214035f, 990u, -771.823f, -904.2852f, -2269.56f },
                    { (byte)4, (ushort)167, (byte)4, "Exile Aurin - Novice", -2.751458f, 3460u, 29.1286f, -853.8716f, -560.188f }
                });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "worldId", "x", "y", "z" },
                values: new object[] { (byte)5, (ushort)167, (byte)3, "Exile Granok - Level 50", 51u, 4074.34f, -797.8368f, -2399.37f });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "rx", "worldId", "x", "y", "z" },
                values: new object[,]
                {
                    { (byte)3, (ushort)167, (byte)3, "Exile Granok- Veteran", 0.317613f, 426u, 4110.71f, -658.6249f, -5145.48f },
                    { (byte)4, (ushort)167, (byte)3, "Exile Granok - Novice", -2.751458f, 3460u, 29.1286f, -853.8716f, -560.188f }
                });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "worldId", "x", "y", "z" },
                values: new object[] { (byte)5, (ushort)167, (byte)1, "Exile Human - Level 50", 51u, 4074.34f, -797.8368f, -2399.37f });

            migrationBuilder.InsertData(
                table: "character_create",
                columns: new[] { "creationStart", "faction", "race", "comment", "rx", "worldId", "x", "y", "z" },
                values: new object[,]
                {
                    { (byte)3, (ushort)167, (byte)1, "Exile Human - Veteran", 0.317613f, 426u, 4110.71f, -658.6249f, -5145.48f },
                    { (byte)3, (ushort)166, (byte)12, "Dominion Mechari - Veteran", -0.45682f, 1387u, -3835.341f, -980.2174f, -6050.524f },
                    { (byte)5, (ushort)166, (byte)12, "Dominion Mechari - Level 50", -0.7632219f, 22u, -3343.58f, -887.4646f, -536.03f }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_create");
        }
    }
}
