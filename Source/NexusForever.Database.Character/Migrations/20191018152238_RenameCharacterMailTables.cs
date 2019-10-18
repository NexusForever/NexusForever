using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Character.Migrations
{
    public partial class RenameCharacterMailTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_mail_character_RecipientId",
                table: "character_mail");

            migrationBuilder.DropForeignKey(
                name: "FK_character_mail_attachment_character_mail_Id",
                table: "character_mail_attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_character_mail_attachment_item_ItemGuid",
                table: "character_mail_attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_mail_attachment",
                table: "character_mail_attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_character_mail",
                table: "character_mail");

            migrationBuilder.RenameTable(
                name: "character_mail_attachment",
                newName: "mail_attachment");

            migrationBuilder.RenameTable(
                name: "character_mail",
                newName: "mail");

            migrationBuilder.RenameIndex(
                name: "IX_character_mail_attachment_ItemGuid",
                table: "mail_attachment",
                newName: "IX_mail_attachment_ItemGuid");

            migrationBuilder.RenameIndex(
                name: "IX_character_mail_attachment_Id",
                table: "mail_attachment",
                newName: "IX_mail_attachment_Id");

            migrationBuilder.RenameIndex(
                name: "IX_character_mail_RecipientId",
                table: "mail",
                newName: "IX_mail_RecipientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mail_attachment",
                table: "mail_attachment",
                columns: new[] { "Id", "Index" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_mail",
                table: "mail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_mail_character_RecipientId",
                table: "mail",
                column: "RecipientId",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mail_attachment_mail_Id",
                table: "mail_attachment",
                column: "Id",
                principalTable: "mail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mail_attachment_item_ItemGuid",
                table: "mail_attachment",
                column: "ItemGuid",
                principalTable: "item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mail_character_RecipientId",
                table: "mail");

            migrationBuilder.DropForeignKey(
                name: "FK_mail_attachment_mail_Id",
                table: "mail_attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_mail_attachment_item_ItemGuid",
                table: "mail_attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mail_attachment",
                table: "mail_attachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mail",
                table: "mail");

            migrationBuilder.RenameTable(
                name: "mail_attachment",
                newName: "character_mail_attachment");

            migrationBuilder.RenameTable(
                name: "mail",
                newName: "character_mail");

            migrationBuilder.RenameIndex(
                name: "IX_mail_attachment_ItemGuid",
                table: "character_mail_attachment",
                newName: "IX_character_mail_attachment_ItemGuid");

            migrationBuilder.RenameIndex(
                name: "IX_mail_attachment_Id",
                table: "character_mail_attachment",
                newName: "IX_character_mail_attachment_Id");

            migrationBuilder.RenameIndex(
                name: "IX_mail_RecipientId",
                table: "character_mail",
                newName: "IX_character_mail_RecipientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_mail_attachment",
                table: "character_mail_attachment",
                columns: new[] { "Id", "Index" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_character_mail",
                table: "character_mail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_character_mail_character_RecipientId",
                table: "character_mail",
                column: "RecipientId",
                principalTable: "character",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_mail_attachment_character_mail_Id",
                table: "character_mail_attachment",
                column: "Id",
                principalTable: "character_mail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_mail_attachment_item_ItemGuid",
                table: "character_mail_attachment",
                column: "ItemGuid",
                principalTable: "item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
