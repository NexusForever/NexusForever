using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    public partial class Guilds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "flags",
                table: "character",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<ulong>(
                name: "guildAffiliation",
                table: "character",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "guild",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    flags = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    type = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    name = table.Column<string>(type: "varchar(30)", nullable: true, defaultValue: ""),
                    leaderId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: true),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()"),
                    deleteTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    originalName = table.Column<string>(type: "varchar(30)", nullable: true),
                    orginialLeaderId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guild_guild_data",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    motd = table.Column<string>(type: "varchar(200)", nullable: true, defaultValue: ""),
                    additionalInfo = table.Column<string>(type: "varchar(400)", nullable: true, defaultValue: ""),
                    backgroundIconPartId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    foregroundIconPartId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    scanLinesPartId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0)
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
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    characterId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    rank = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    note = table.Column<string>(type: "varchar(32)", nullable: true, defaultValue: "")
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
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    index = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    name = table.Column<string>(type: "varchar(16)", nullable: true, defaultValue: ""),
                    permission = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    bankWithdrawPermission = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    moneyWithdrawalLimit = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    repairLimit = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul)
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
                name: "flags",
                table: "character");

            migrationBuilder.DropColumn(
                name: "guildAffiliation",
                table: "character");
        }
    }
}
