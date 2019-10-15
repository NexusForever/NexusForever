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
                    Id = table.Column<uint>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "varchar(128)", nullable: false),
                    S = table.Column<string>(type: "varchar(32)", nullable: false),
                    V = table.Column<string>(type: "varchar(512)", nullable: false),
                    GameToken = table.Column<string>(type: "varchar(32)", nullable: true),
                    SessionKey = table.Column<string>(type: "varchar(32)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "server",
                columns: table => new
                {
                    Id = table.Column<byte>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false),
                    Host = table.Column<string>(type: "varchar(64)", nullable: false),
                    Port = table.Column<ushort>(nullable: false),
                    Type = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "server_message",
                columns: table => new
                {
                    Index = table.Column<byte>(nullable: false),
                    Language = table.Column<byte>(nullable: false),
                    Message = table.Column<string>(type: "varchar(256)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_message", x => new { x.Index, x.Language });
                });

            migrationBuilder.CreateTable(
                name: "account_costume_unlock",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    ItemId = table.Column<uint>(nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_costume_unlock", x => new { x.Id, x.ItemId });
                    table.ForeignKey(
                        name: "FK_account_costume_unlock_account_Id",
                        column: x => x.Id,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_currency",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    CurrencyId = table.Column<byte>(nullable: false),
                    Amount = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_currency", x => new { x.Id, x.CurrencyId });
                    table.ForeignKey(
                        name: "FK_account_currency_account_Id",
                        column: x => x.Id,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_entitlement",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    EntitlementId = table.Column<byte>(nullable: false),
                    Amount = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_entitlement", x => new { x.Id, x.EntitlementId });
                    table.ForeignKey(
                        name: "FK_account_entitlement_account_Id",
                        column: x => x.Id,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_generic_unlock",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    Entry = table.Column<uint>(nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_generic_unlock", x => new { x.Id, x.Entry });
                    table.ForeignKey(
                        name: "FK_account_generic_unlock_account_Id",
                        column: x => x.Id,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_keybinding",
                columns: table => new
                {
                    Id = table.Column<uint>(nullable: false),
                    InputActionId = table.Column<ushort>(nullable: false),
                    DeviceEnum00 = table.Column<uint>(nullable: false),
                    DeviceEnum01 = table.Column<uint>(nullable: false),
                    DeviceEnum02 = table.Column<uint>(nullable: false),
                    Code00 = table.Column<uint>(nullable: false),
                    Code01 = table.Column<uint>(nullable: false),
                    Code02 = table.Column<uint>(nullable: false),
                    MetaKeys00 = table.Column<uint>(nullable: false),
                    MetaKeys01 = table.Column<uint>(nullable: false),
                    MetaKeys02 = table.Column<uint>(nullable: false),
                    EventTypeEnum00 = table.Column<uint>(nullable: false),
                    EventTypeEnum01 = table.Column<uint>(nullable: false),
                    EventTypeEnum02 = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_keybinding", x => new { x.Id, x.InputActionId });
                    table.ForeignKey(
                        name: "FK_account_keybinding_account_Id",
                        column: x => x.Id,
                        principalTable: "account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "server",
                columns: new[] { "Id", "Host", "Name", "Port", "Type" },
                values: new object[] { (byte)1, "127.0.0.1", "NexusForever", (ushort)24000, (byte)0 });

            migrationBuilder.CreateIndex(
                name: "IX_account_Email",
                table: "account",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_account_GameToken",
                table: "account",
                column: "GameToken");

            migrationBuilder.CreateIndex(
                name: "IX_account_SessionKey",
                table: "account",
                column: "SessionKey");
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
