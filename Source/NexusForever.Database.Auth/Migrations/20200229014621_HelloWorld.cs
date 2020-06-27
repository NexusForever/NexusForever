using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Auth.Migrations
{
    public partial class HelloWorld : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(128)", nullable: false, defaultValue: ""),
                    s = table.Column<string>(type: "varchar(32)", nullable: false, defaultValue: ""),
                    v = table.Column<string>(type: "varchar(512)", nullable: false, defaultValue: ""),
                    gameToken = table.Column<string>(type: "varchar(32)", nullable: false, defaultValue: ""),
                    sessionKey = table.Column<string>(type: "varchar(32)", nullable: false, defaultValue: ""),
                    createTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "server",
                columns: table => new
                {
                    id = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(64)", nullable: false, defaultValue: "NexusForever"),
                    host = table.Column<string>(type: "varchar(64)", nullable: false, defaultValue: "127.0.0.1"),
                    port = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)24000),
                    type = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "server_message",
                columns: table => new
                {
                    index = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    language = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    message = table.Column<string>(type: "varchar(256)", nullable: false, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.index, x.language });
                });

            migrationBuilder.CreateTable(
                name: "account_costume_unlock",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    itemId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.itemId });
                    table.ForeignKey(
                        name: "FK__account_costume_item_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_currency",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    currencyId = table.Column<byte>(type: "tinyint(4) unsigned", nullable: false, defaultValue: (byte)0),
                    amount = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.currencyId });
                    table.ForeignKey(
                        name: "FK__account_currency_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_entitlement",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    entitlementId = table.Column<byte>(type: "tinyint(3) unsigned", nullable: false, defaultValue: (byte)0),
                    amount = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.entitlementId });
                    table.ForeignKey(
                        name: "FK__account_entitlement_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_generic_unlock",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    entry = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.entry });
                    table.ForeignKey(
                        name: "FK__account_generic_unlock_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_keybinding",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    inputActionId = table.Column<ushort>(type: "smallint(5) unsigned", nullable: false, defaultValue: (ushort)0),
                    deviceEnum00 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    deviceEnum01 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    deviceEnum02 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    code00 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    code01 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    code02 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    metaKeys00 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    metaKeys01 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    metaKeys02 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    eventTypeEnum00 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    eventTypeEnum01 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    eventTypeEnum02 = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.inputActionId });
                    table.ForeignKey(
                        name: "FK__account_keybinding_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "server",
                columns: new[] { "id", "host", "name", "port" },
                values: new object[] { (byte)1, "127.0.0.1", "NexusForever", (ushort)24000 });

            migrationBuilder.InsertData(
                table: "server_message",
                columns: new[] { "index", "language", "message" },
                values: new object[] { (byte)0, (byte)1, @"Willkommen auf diesem NexusForever server!
Besuch: https://github.com/NexusForever/NexusForever" });

            migrationBuilder.InsertData(
                table: "server_message",
                columns: new[] { "index", "language", "message" },
                values: new object[] { (byte)0, (byte)0, @"Welcome to this NexusForever server!
Visit: https://github.com/NexusForever/NexusForever" });

            migrationBuilder.CreateIndex(
                name: "email",
                table: "account",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "gameToken",
                table: "account",
                column: "gameToken");

            migrationBuilder.CreateIndex(
                name: "sessionKey",
                table: "account",
                column: "sessionKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_costume_unlock");

            migrationBuilder.DropTable(
                name: "account_currency");

            migrationBuilder.DropTable(
                name: "account_entitlement");

            migrationBuilder.DropTable(
                name: "account_generic_unlock");

            migrationBuilder.DropTable(
                name: "account_keybinding");

            migrationBuilder.DropTable(
                name: "server");

            migrationBuilder.DropTable(
                name: "server_message");

            migrationBuilder.DropTable(
                name: "account");
        }
    }
}
