using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Auth.Migrations
{
    public partial class AccountRBAC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permission",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    name = table.Column<string>(type: "varchar(64)", nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    name = table.Column<string>(type: "varchar(64)", nullable: true, defaultValue: ""),
                    flags = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "account_permission",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    permissionId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK__account_permission_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__account_permission_permission_id__permission_id",
                        column: x => x.permissionId,
                        principalTable: "permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_role",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    roleId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_role", x => x.id);
                    table.ForeignKey(
                        name: "FK__account_role_id__account_id",
                        column: x => x.id,
                        principalTable: "account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__account_role_role_id__role_id",
                        column: x => x.roleId,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    id = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u),
                    permissionId = table.Column<uint>(type: "int(10) unsigned", nullable: false, defaultValue: 0u)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK__role_permission_id__role_id",
                        column: x => x.id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__role_permission_permission_id__permission_id",
                        column: x => x.permissionId,
                        principalTable: "permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1u, "Category: Account" },
                    { 79u, "Command: QuestAchieveObjective" },
                    { 78u, "Command: QuestAchieve" },
                    { 77u, "Command: QuestAdd" },
                    { 76u, "Category: Quest" },
                    { 75u, "Command: PetUnlockFlair" },
                    { 74u, "Category: Pet" },
                    { 73u, "Command: PathXP" },
                    { 72u, "Command: PathActivate" },
                    { 71u, "Command: PathUnlock" },
                    { 70u, "Category: Path" },
                    { 69u, "Command: MovementGeneratorRandom" },
                    { 68u, "Command: MovementGeneratorDirect" },
                    { 67u, "Category: MovementGenerator" },
                    { 66u, "Command: MovementSplineLaunch" },
                    { 65u, "Command: MovementSplineClear" },
                    { 64u, "Command: MovementSplineAdd" },
                    { 63u, "Category: MovementSpline" },
                    { 62u, "Category: Movement" },
                    { 59u, "Category: Item" },
                    { 80u, "Command: QuestObjective" },
                    { 57u, "Command: HouseDecorLookup" },
                    { 81u, "Command: QuestKill" },
                    { 8u, "Category: RBACAccount" },
                    { 91u, "Command: TitleNone" },
                    { 90u, "Command: TitleAll" },
                    { 89u, "Command: TitleRevoke" },
                    { 88u, "Command: TitleAdd" },
                    { 87u, "Category: Title" },
                    { 53u, "Command: TeleportName" },
                    { 52u, "Command: TeleportLocation" },
                    { 51u, "Command: TeleportCoordinates" },
                    { 50u, "Category: Teleport" },
                    { 86u, "Command: SpellResetCooldown" },
                    { 85u, "Command: SpellCast" },
                    { 84u, "Command: SpellAdd" },
                    { 83u, "Category: Spell" },
                    { 14u, "Command: RBACAccountRoleRevoke" },
                    { 13u, "Command: RBACAccountRoleGrant" },
                    { 12u, "Category: RBACAccountRole" },
                    { 11u, "Command: RBACAccountPermissionRevoke" },
                    { 10u, "Command: RBACAccountPermissionGrant" },
                    { 9u, "Category: RBACAccountPermission" },
                    { 7u, "Category: RBAC" },
                    { 56u, "Command: HouseDecorAdd" },
                    { 6u, "Command: ItemAdd" },
                    { 58u, "Command: HouseTeleport" },
                    { 55u, "Category: HouseDecor" },
                    { 29u, "Command: CurrencyCharacterList" },
                    { 28u, "Command: CurrencyCharacterAdd" },
                    { 27u, "Category: CurrencyCharacter" },
                    { 26u, "Command: CurrencyAccountList" },
                    { 25u, "Command: CurrencyAccountAdd" },
                    { 24u, "Category: CurrencyAccount" },
                    { 23u, "Category: Currency" },
                    { 5u, "Command: CharacterSave" },
                    { 22u, "Command: CharacterLevel" },
                    { 21u, "Command: CharacterXP" },
                    { 20u, "Category: Character" },
                    { 19u, "Command: BroadcastMessage" },
                    { 18u, "Category: Broadcast" },
                    { 16u, "Command: AchievementGrant" },
                    { 17u, "Command: AchievementUpdate" },
                    { 15u, "Category: Achievement" },
                    { 3u, "Command: AccountDelete" },
                    { 2u, "Command: AccountCreate" },
                    { 31u, "Command: DisableInfo" },
                    { 32u, "Command: DisableReload" },
                    { 30u, "Category: Disable" },
                    { 34u, "Command: DoorOpen" },
                    { 54u, "Category: House" },
                    { 4u, "Category: Help" },
                    { 49u, "Command: GenericList" },
                    { 48u, "Command: GenericUnlockAll" },
                    { 47u, "Command: GenericUnlock" },
                    { 33u, "Category: Door" },
                    { 61u, "Command: EntityModifyDisplayInfo" },
                    { 60u, "Category: EntityModify" },
                    { 45u, "Command: EntityProperties" },
                    { 46u, "Category: Generic" },
                    { 43u, "Category: Entity" },
                    { 39u, "Command: EntitlementCharacterList" },
                    { 38u, "Command: EntitlementCharacterAdd" },
                    { 37u, "Category: EntitlementCharacter" },
                    { 42u, "Command: EntitlementAccountList" },
                    { 41u, "Command: EntitlementAccountAdd" },
                    { 40u, "Category: EntitlementAccount" },
                    { 36u, "Category: Entitlement" },
                    { 44u, "Command: EntityInfo" },
                    { 35u, "Command: DoorClose" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "flags", "name" },
                values: new object[,]
                {
                    { 1u, 1u, "Player" },
                    { 2u, 1u, "GameMaster" },
                    { 3u, 2u, "Administrator" },
                    { 4u, 2u, "Console" },
                    { 5u, 2u, "WebSocket" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_permission_permissionId",
                table: "account_permission",
                column: "permissionId");

            migrationBuilder.CreateIndex(
                name: "IX_account_role_roleId",
                table: "account_role",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_permissionId",
                table: "role_permission",
                column: "permissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_permission");

            migrationBuilder.DropTable(
                name: "account_role");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "permission");
        }
    }
}
