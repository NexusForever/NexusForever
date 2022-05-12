using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Auth.Migrations
{
    public partial class StorePurchasing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account_store_transaction",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    transactionId = table.Column<uint>(type: "int(20) unsigned", nullable: false, defaultValue: 0u),
                    name = table.Column<string>(type: "varchar(256)", nullable: true, defaultValue: ""),
                    currencyType = table.Column<short>(type: "smallint(5)", nullable: false, defaultValue: (short)6),
                    cost = table.Column<float>(type: "float", nullable: false, defaultValue: 0f),
                    transactionDateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_store_transaction", x => new { x.id, x.transactionId });
                    table.ForeignKey(
                        name: "FK__account_store_transaction_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_store_transaction");
        }
    }
}
