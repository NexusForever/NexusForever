using Microsoft.EntityFrameworkCore.Migrations;

namespace NexusForever.Database.Auth.Migrations
{
    public partial class ItemLookupPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 1u,
                column: "name",
                value: "Category: Account");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 2u,
                column: "name",
                value: "Command: AccountCreate");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 3u,
                column: "name",
                value: "Command: AccountDelete");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 4u,
                column: "name",
                value: "Category: Help");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 5u,
                column: "name",
                value: "Command: CharacterSave");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 6u,
                column: "name",
                value: "Command: ItemAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 7u,
                column: "name",
                value: "Category: RBAC");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 8u,
                column: "name",
                value: "Category: RBACAccount");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 9u,
                column: "name",
                value: "Category: RBACAccountPermission");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 10u,
                column: "name",
                value: "Command: RBACAccountPermissionGrant");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 11u,
                column: "name",
                value: "Command: RBACAccountPermissionRevoke");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 12u,
                column: "name",
                value: "Category: RBACAccountRole");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 13u,
                column: "name",
                value: "Command: RBACAccountRoleGrant");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 14u,
                column: "name",
                value: "Command: RBACAccountRoleRevoke");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 15u,
                column: "name",
                value: "Category: Achievement");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 16u,
                column: "name",
                value: "Command: AchievementGrant");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 17u,
                column: "name",
                value: "Command: AchievementUpdate");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 18u,
                column: "name",
                value: "Category: Broadcast");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 19u,
                column: "name",
                value: "Command: BroadcastMessage");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 20u,
                column: "name",
                value: "Category: Character");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 21u,
                column: "name",
                value: "Command: CharacterXP");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 22u,
                column: "name",
                value: "Command: CharacterLevel");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 23u,
                column: "name",
                value: "Category: Currency");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 24u,
                column: "name",
                value: "Category: CurrencyAccount");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 25u,
                column: "name",
                value: "Command: CurrencyAccountAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 26u,
                column: "name",
                value: "Command: CurrencyAccountList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 27u,
                column: "name",
                value: "Category: CurrencyCharacter");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 28u,
                column: "name",
                value: "Command: CurrencyCharacterAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 29u,
                column: "name",
                value: "Command: CurrencyCharacterList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 30u,
                column: "name",
                value: "Category: Disable");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 31u,
                column: "name",
                value: "Command: DisableInfo");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 32u,
                column: "name",
                value: "Command: DisableReload");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 33u,
                column: "name",
                value: "Category: Door");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 34u,
                column: "name",
                value: "Command: DoorOpen");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 35u,
                column: "name",
                value: "Command: DoorClose");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 36u,
                column: "name",
                value: "Category: Entitlement");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 37u,
                column: "name",
                value: "Category: EntitlementCharacter");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 38u,
                column: "name",
                value: "Command: EntitlementCharacterAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 39u,
                column: "name",
                value: "Command: EntitlementCharacterList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 40u,
                column: "name",
                value: "Category: EntitlementAccount");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 41u,
                column: "name",
                value: "Command: EntitlementAccountAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 42u,
                column: "name",
                value: "Command: EntitlementAccountList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 43u,
                column: "name",
                value: "Category: Entity");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 44u,
                column: "name",
                value: "Command: EntityInfo");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 45u,
                column: "name",
                value: "Command: EntityProperties");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 46u,
                column: "name",
                value: "Category: Generic");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 47u,
                column: "name",
                value: "Command: GenericUnlock");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 48u,
                column: "name",
                value: "Command: GenericUnlockAll");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 49u,
                column: "name",
                value: "Command: GenericList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 50u,
                column: "name",
                value: "Category: Teleport");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 51u,
                column: "name",
                value: "Command: TeleportCoordinates");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 52u,
                column: "name",
                value: "Command: TeleportLocation");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 53u,
                column: "name",
                value: "Command: TeleportName");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 54u,
                column: "name",
                value: "Category: House");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 55u,
                column: "name",
                value: "Category: HouseDecor");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 56u,
                column: "name",
                value: "Command: HouseDecorAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 57u,
                column: "name",
                value: "Command: HouseDecorLookup");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 58u,
                column: "name",
                value: "Command: HouseTeleport");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 59u,
                column: "name",
                value: "Category: Item");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 60u,
                column: "name",
                value: "Category: EntityModify");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 61u,
                column: "name",
                value: "Command: EntityModifyDisplayInfo");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 62u,
                column: "name",
                value: "Category: Movement");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 63u,
                column: "name",
                value: "Category: MovementSpline");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 64u,
                column: "name",
                value: "Command: MovementSplineAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 65u,
                column: "name",
                value: "Command: MovementSplineClear");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 66u,
                column: "name",
                value: "Command: MovementSplineLaunch");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 67u,
                column: "name",
                value: "Category: MovementGenerator");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 68u,
                column: "name",
                value: "Command: MovementGeneratorDirect");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 69u,
                column: "name",
                value: "Command: MovementGeneratorRandom");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 70u,
                column: "name",
                value: "Category: Path");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 71u,
                column: "name",
                value: "Command: PathUnlock");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 72u,
                column: "name",
                value: "Command: PathActivate");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 73u,
                column: "name",
                value: "Command: PathXP");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 74u,
                column: "name",
                value: "Category: Pet");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 75u,
                column: "name",
                value: "Command: PetUnlockFlair");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 76u,
                column: "name",
                value: "Category: Quest");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 77u,
                column: "name",
                value: "Command: QuestAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 78u,
                column: "name",
                value: "Command: QuestAchieve");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 79u,
                column: "name",
                value: "Command: QuestAchieveObjective");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 80u,
                column: "name",
                value: "Command: QuestObjective");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 81u,
                column: "name",
                value: "Command: QuestKill");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 83u,
                column: "name",
                value: "Category: Spell");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 84u,
                column: "name",
                value: "Command: SpellAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 85u,
                column: "name",
                value: "Command: SpellCast");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 86u,
                column: "name",
                value: "Command: SpellResetCooldown");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 87u,
                column: "name",
                value: "Category: Title");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 88u,
                column: "name",
                value: "Command: TitleAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 89u,
                column: "name",
                value: "Command: TitleRevoke");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 90u,
                column: "name",
                value: "Command: TitleAll");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 91u,
                column: "name",
                value: "Command: TitleNone");

            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "name" },
                values: new object[] { 92u, "Command: ItemLookup" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1u,
                columns: new[] { "flags", "name" },
                values: new object[] { 1u, "Player" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2u,
                columns: new[] { "flags", "name" },
                values: new object[] { 1u, "GameMaster" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 3u,
                columns: new[] { "flags", "name" },
                values: new object[] { 2u, "Administrator" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 4u,
                columns: new[] { "flags", "name" },
                values: new object[] { 2u, "Console" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 5u,
                columns: new[] { "flags", "name" },
                values: new object[] { 2u, "WebSocket" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permission",
                keyColumn: "id",
                keyValue: 92u);

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 1u,
                column: "name",
                value: "Category: Account");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 2u,
                column: "name",
                value: "Command: AccountCreate");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 3u,
                column: "name",
                value: "Command: AccountDelete");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 4u,
                column: "name",
                value: "Category: Help");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 5u,
                column: "name",
                value: "Command: CharacterSave");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 6u,
                column: "name",
                value: "Command: ItemAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 7u,
                column: "name",
                value: "Category: RBAC");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 8u,
                column: "name",
                value: "Category: RBACAccount");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 9u,
                column: "name",
                value: "Category: RBACAccountPermission");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 10u,
                column: "name",
                value: "Command: RBACAccountPermissionGrant");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 11u,
                column: "name",
                value: "Command: RBACAccountPermissionRevoke");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 12u,
                column: "name",
                value: "Category: RBACAccountRole");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 13u,
                column: "name",
                value: "Command: RBACAccountRoleGrant");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 14u,
                column: "name",
                value: "Command: RBACAccountRoleRevoke");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 15u,
                column: "name",
                value: "Category: Achievement");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 16u,
                column: "name",
                value: "Command: AchievementGrant");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 17u,
                column: "name",
                value: "Command: AchievementUpdate");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 18u,
                column: "name",
                value: "Category: Broadcast");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 19u,
                column: "name",
                value: "Command: BroadcastMessage");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 20u,
                column: "name",
                value: "Category: Character");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 21u,
                column: "name",
                value: "Command: CharacterXP");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 22u,
                column: "name",
                value: "Command: CharacterLevel");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 23u,
                column: "name",
                value: "Category: Currency");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 24u,
                column: "name",
                value: "Category: CurrencyAccount");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 25u,
                column: "name",
                value: "Command: CurrencyAccountAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 26u,
                column: "name",
                value: "Command: CurrencyAccountList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 27u,
                column: "name",
                value: "Category: CurrencyCharacter");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 28u,
                column: "name",
                value: "Command: CurrencyCharacterAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 29u,
                column: "name",
                value: "Command: CurrencyCharacterList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 30u,
                column: "name",
                value: "Category: Disable");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 31u,
                column: "name",
                value: "Command: DisableInfo");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 32u,
                column: "name",
                value: "Command: DisableReload");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 33u,
                column: "name",
                value: "Category: Door");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 34u,
                column: "name",
                value: "Command: DoorOpen");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 35u,
                column: "name",
                value: "Command: DoorClose");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 36u,
                column: "name",
                value: "Category: Entitlement");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 37u,
                column: "name",
                value: "Category: EntitlementCharacter");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 38u,
                column: "name",
                value: "Command: EntitlementCharacterAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 39u,
                column: "name",
                value: "Command: EntitlementCharacterList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 40u,
                column: "name",
                value: "Category: EntitlementAccount");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 41u,
                column: "name",
                value: "Command: EntitlementAccountAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 42u,
                column: "name",
                value: "Command: EntitlementAccountList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 43u,
                column: "name",
                value: "Category: Entity");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 44u,
                column: "name",
                value: "Command: EntityInfo");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 45u,
                column: "name",
                value: "Command: EntityProperties");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 46u,
                column: "name",
                value: "Category: Generic");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 47u,
                column: "name",
                value: "Command: GenericUnlock");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 48u,
                column: "name",
                value: "Command: GenericUnlockAll");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 49u,
                column: "name",
                value: "Command: GenericList");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 50u,
                column: "name",
                value: "Category: Teleport");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 51u,
                column: "name",
                value: "Command: TeleportCoordinates");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 52u,
                column: "name",
                value: "Command: TeleportLocation");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 53u,
                column: "name",
                value: "Command: TeleportName");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 54u,
                column: "name",
                value: "Category: House");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 55u,
                column: "name",
                value: "Category: HouseDecor");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 56u,
                column: "name",
                value: "Command: HouseDecorAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 57u,
                column: "name",
                value: "Command: HouseDecorLookup");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 58u,
                column: "name",
                value: "Command: HouseTeleport");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 59u,
                column: "name",
                value: "Category: Item");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 60u,
                column: "name",
                value: "Category: EntityModify");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 61u,
                column: "name",
                value: "Command: EntityModifyDisplayInfo");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 62u,
                column: "name",
                value: "Category: Movement");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 63u,
                column: "name",
                value: "Category: MovementSpline");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 64u,
                column: "name",
                value: "Command: MovementSplineAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 65u,
                column: "name",
                value: "Command: MovementSplineClear");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 66u,
                column: "name",
                value: "Command: MovementSplineLaunch");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 67u,
                column: "name",
                value: "Category: MovementGenerator");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 68u,
                column: "name",
                value: "Command: MovementGeneratorDirect");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 69u,
                column: "name",
                value: "Command: MovementGeneratorRandom");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 70u,
                column: "name",
                value: "Category: Path");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 71u,
                column: "name",
                value: "Command: PathUnlock");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 72u,
                column: "name",
                value: "Command: PathActivate");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 73u,
                column: "name",
                value: "Command: PathXP");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 74u,
                column: "name",
                value: "Category: Pet");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 75u,
                column: "name",
                value: "Command: PetUnlockFlair");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 76u,
                column: "name",
                value: "Category: Quest");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 77u,
                column: "name",
                value: "Command: QuestAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 78u,
                column: "name",
                value: "Command: QuestAchieve");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 79u,
                column: "name",
                value: "Command: QuestAchieveObjective");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 80u,
                column: "name",
                value: "Command: QuestObjective");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 81u,
                column: "name",
                value: "Command: QuestKill");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 83u,
                column: "name",
                value: "Category: Spell");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 84u,
                column: "name",
                value: "Command: SpellAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 85u,
                column: "name",
                value: "Command: SpellCast");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 86u,
                column: "name",
                value: "Command: SpellResetCooldown");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 87u,
                column: "name",
                value: "Category: Title");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 88u,
                column: "name",
                value: "Command: TitleAdd");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 89u,
                column: "name",
                value: "Command: TitleRevoke");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 90u,
                column: "name",
                value: "Command: TitleAll");

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: 91u,
                column: "name",
                value: "Command: TitleNone");

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 1u,
                columns: new[] { "flags", "name" },
                values: new object[] { 1u, "Player" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 2u,
                columns: new[] { "flags", "name" },
                values: new object[] { 1u, "GameMaster" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 3u,
                columns: new[] { "flags", "name" },
                values: new object[] { 2u, "Administrator" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 4u,
                columns: new[] { "flags", "name" },
                values: new object[] { 2u, "Console" });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: 5u,
                columns: new[] { "flags", "name" },
                values: new object[] { 2u, "WebSocket" });
        }
    }
}
