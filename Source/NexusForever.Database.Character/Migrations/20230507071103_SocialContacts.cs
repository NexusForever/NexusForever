using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Character.Migrations
{
    /// <inheritdoc />
    public partial class SocialContacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "character_contact",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    ownerId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    contactId = table.Column<ulong>(type: "bigint(20) unsigned", nullable: false, defaultValue: 0ul),
                    type = table.Column<uint>(type: "int(3) unsigned", nullable: false, defaultValue: 0u),
                    inviteMessage = table.Column<string>(type: "varchar(100)", nullable: true, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    privateNote = table.Column<string>(type: "varchar(100)", nullable: true, defaultValueSql: "''")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    accepted = table.Column<byte>(type: "tinyint(8) unsigned", nullable: false, defaultValue: (byte)0),
                    requestTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "current_timestamp()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.id, x.ownerId, x.contactId });
                    table.ForeignKey(
                        name: "FK__character_contact_id__character_id",
                        column: x => x.ownerId,
                        principalTable: "character",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "contactGuid",
                table: "character_contact",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_character_contact_ownerId",
                table: "character_contact",
                column: "ownerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_contact");
        }
    }
}
