using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    public partial class Guilds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "guildAffiliation",
                table: "character",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.AddColumn<byte>(
                name: "guildHolomarkMask",
                table: "character",
                nullable: false,
                defaultValueSql: "'0'");

            migrationBuilder.CreateTable(
                name: "guild",
                columns: table => new
                {
                    id = table.Column<ulong>(nullable: false, defaultValueSql: "'0'"),
                    type = table.Column<byte>(nullable: false, defaultValueSql: "'0'"),
                    name = table.Column<string>(nullable: true),
                    leaderId = table.Column<ulong>(nullable: false, defaultValueSql: "'0'"),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guild_guild_data",
                columns: table => new
                {
                    id = table.Column<ulong>(nullable: false, defaultValueSql: "'0'"),
                    taxes = table.Column<uint>(nullable: false, defaultValueSql: "'0'"),
                    motd = table.Column<string>(type: "varchar(200)", nullable: true, defaultValueSql: "''"),
                    additionalInfo = table.Column<string>(type: "varchar(200)", nullable: true, defaultValueSql: "''"),
                    backgroundIconPartId = table.Column<ushort>(nullable: false, defaultValueSql: "'0'"),
                    foregroundIconPartId = table.Column<ushort>(nullable: false, defaultValueSql: "'0'"),
                    scanLinesPartId = table.Column<ushort>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                    table.ForeignKey(
                        name: "FK__guild_guild_data_id__guild_id",
                        column: x => x.id,
                        principalTable: "guild",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guild_member",
                columns: table => new
                {
                    id = table.Column<ulong>(nullable: false, defaultValueSql: "'0'"),
                    characterId = table.Column<ulong>(nullable: false, defaultValueSql: "'0'"),
                    rank = table.Column<byte>(nullable: false),
                    note = table.Column<string>(type: "varchar(50)", nullable: true, defaultValueSql: "''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.characterId });
                    table.ForeignKey(
                        name: "FK__guild_member_id__guild_id",
                        column: x => x.id,
                        principalTable: "guild",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "guild_rank",
                columns: table => new
                {
                    id = table.Column<ulong>(nullable: false, defaultValueSql: "'0'"),
                    index = table.Column<byte>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    permission = table.Column<int>(nullable: false),
                    bankWithdrawPermission = table.Column<ulong>(nullable: false, defaultValueSql: "'0'"),
                    moneyWithdrawalLimit = table.Column<ulong>(nullable: false, defaultValueSql: "'0'"),
                    repairLimit = table.Column<ulong>(nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.index });
                    table.ForeignKey(
                        name: "FK__guild_rank_id__guild_id",
                        column: x => x.id,
                        principalTable: "guild",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guild_guild_data");

            migrationBuilder.DropTable(
                name: "guild_member");

            migrationBuilder.DropTable(
                name: "guild_rank");

            migrationBuilder.DropTable(
                name: "guild");

            migrationBuilder.DropColumn(
                name: "guildAffiliation",
                table: "character");

            migrationBuilder.DropColumn(
                name: "guildHolomarkMask",
                table: "character");
        }
    }
}
