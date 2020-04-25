using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Auth.Migrations
{
    public partial class AccountItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account_item",
                columns: table => new
                {
                    entry = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    accountId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    itemId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.entry, x.accountId });
                    table.ForeignKey(
                        name: "FK__account_item_accountId__account_id",
                        column: x => x.accountId,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_item_cooldown",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    cooldownGroupId = table.Column<uint>(type: "int(10) unsigned", nullable: false),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "current_timestamp()"),
                    duration = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.cooldownGroupId });
                    table.ForeignKey(
                        name: "FK__account_item_cooldown_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_item_accountId",
                table: "account_item",
                column: "accountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_item");

            migrationBuilder.DropTable(
                name: "account_item_cooldown");
        }
    }
}
