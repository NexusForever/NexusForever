using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Auth.Migrations
{
    public partial class RBACPrimaryKeys : Migration
    {
        // This migration was manually edited due to a bug #678 in Pomelo.EntityFrameworkCore.MySql
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK__account_permission_id__account_id",
                table: "account_permission");

            migrationBuilder.DropForeignKey(
                name: "FK__account_permission_permission_id__permission_id",
                table: "account_permission");

            migrationBuilder.DropForeignKey(
                name: "FK__account_role_id__account_id",
                table: "account_role");

            migrationBuilder.DropForeignKey(
                name: "FK__account_role_role_id__role_id",
                table: "account_role");

            migrationBuilder.DropForeignKey(
                name: "FK__role_permission_id__role_id",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "FK__role_permission_permission_id__permission_id",
                table: "role_permission");

            // -------------------------------------------------------------

            migrationBuilder.DropPrimaryKey(
                name: "PK_role_permission",
                table: "role_permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_account_role",
                table: "account_role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_account_permission",
                table: "account_permission");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role_permission",
                table: "role_permission",
                columns: new[] { "id", "permissionId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_account_role",
                table: "account_role",
                columns: new[] { "id", "roleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_account_permission",
                table: "account_permission",
                columns: new[] { "id", "permissionId" });

            // -------------------------------------------------------------

            migrationBuilder.AddForeignKey(
                name: "FK__account_permission_id__account_id",
                table: "account_permission",
                column: "id",
                principalTable: "account",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__account_permission_permission_id__permission_id",
                table: "account_permission",
                column: "permissionId",
                principalTable: "permission",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__account_role_id__account_id",
                table: "account_role",
                column: "id",
                principalTable: "account",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__account_role_role_id__role_id",
                table: "account_role",
                column: "roleId",
                principalTable: "role",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__role_permission_id__role_id",
                table: "role_permission",
                column: "id",
                principalTable: "role",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__role_permission_permission_id__permission_id",
                table: "role_permission",
                column: "permissionId",
                principalTable: "permission",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK__account_permission_id__account_id",
                table: "account_permission");

            migrationBuilder.DropForeignKey(
                name: "FK__account_permission_permission_id__permission_id",
                table: "account_permission");

            migrationBuilder.DropForeignKey(
                name: "FK__account_role_id__account_id",
                table: "account_role");

            migrationBuilder.DropForeignKey(
                name: "FK__account_role_role_id__role_id",
                table: "account_role");

            migrationBuilder.DropForeignKey(
                name: "FK__role_permission_id__role_id",
                table: "role_permission");

            migrationBuilder.DropForeignKey(
                name: "FK__role_permission_permission_id__permission_id",
                table: "role_permission");

            // -------------------------------------------------------------

            migrationBuilder.DropPrimaryKey(
                name: "PK_role_permission",
                table: "role_permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_account_role",
                table: "account_role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_account_permission",
                table: "account_permission");

            migrationBuilder.AddPrimaryKey(
                name: "PK_role_permission",
                table: "role_permission",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_account_role",
                table: "account_role",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_account_permission",
                table: "account_permission",
                column: "id");

            // -------------------------------------------------------------

            migrationBuilder.AddForeignKey(
                name: "FK__account_permission_id__account_id",
                table: "account_permission",
                column: "id",
                principalTable: "account",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__account_permission_permission_id__permission_id",
                table: "account_permission",
                column: "permissionId",
                principalTable: "permission",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__account_role_id__account_id",
                table: "account_role",
                column: "id",
                principalTable: "account",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__account_role_role_id__role_id",
                table: "account_role",
                column: "roleId",
                principalTable: "role",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__role_permission_id__role_id",
                table: "role_permission",
                column: "id",
                principalTable: "role",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__role_permission_permission_id__permission_id",
                table: "role_permission",
                column: "permissionId",
                principalTable: "permission",
                principalColumn: "id",
                onUpdate: ReferentialAction.Restrict,
                onDelete: ReferentialAction.Cascade);*/
        }
    }
}
