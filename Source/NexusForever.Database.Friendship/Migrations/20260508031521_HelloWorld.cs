using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusForever.Database.Friendship.Migrations
{
    /// <inheritdoc />
    public partial class HelloWorld : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InternalMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Type = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Payload = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalMessage", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "account",
                columns: table => new
                {
                    accountId = table.Column<uint>(type: "int unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nickname = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    presence = table.Column<int>(type: "int", nullable: false),
                    blockAccountFriendRequests = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    invitePrivilegesSuspended = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    activeCharacterId = table.Column<ulong>(type: "bigint unsigned", nullable: true),
                    activeRealmId = table.Column<ushort>(type: "smallint unsigned", nullable: true),
                    lastOnline = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account", x => x.accountId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character",
                columns: table => new
                {
                    characterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    realmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    realmName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    race = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    @class = table.Column<byte>(name: "class", type: "tinyint unsigned", nullable: false),
                    path = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    faction = table.Column<uint>(type: "int unsigned", nullable: false),
                    worldZoneId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    worldId = table.Column<uint>(type: "int unsigned", nullable: false),
                    lastOnline = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character", x => new { x.characterId, x.realmId });
                    table.ForeignKey(
                        name: "FK_character_account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "friendship_account",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    inviterAccountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    inviteeAccountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendship_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendship_account_account_inviteeAccountId",
                        column: x => x.inviteeAccountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_friendship_account_account_inviterAccountId",
                        column: x => x.inviterAccountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "friendship_account_invite",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    inviteeAccountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    inviterAccountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    seen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expiration = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendship_account_invite", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendship_account_invite_account_inviteeAccountId",
                        column: x => x.inviteeAccountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_friendship_account_invite_account_inviterAccountId",
                        column: x => x.inviterAccountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character_stat",
                columns: table => new
                {
                    characterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    realmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    stat = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    value = table.Column<float>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_stat", x => new { x.characterId, x.realmId, x.stat });
                    table.ForeignKey(
                        name: "FK_character_stat_character_characterId_realmId",
                        columns: x => new { x.characterId, x.realmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "friendship",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    inviterCharacterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    inviterRealmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    friendCharacterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    friendRealmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendship", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendship_character_friendCharacterId_friendRealmId",
                        columns: x => new { x.friendCharacterId, x.friendRealmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_friendship_character_inviterCharacterId_inviterRealmId",
                        columns: x => new { x.inviterCharacterId, x.inviterRealmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "friendship_invite",
                columns: table => new
                {
                    id = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    inviteeCharacterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    inviteeRealmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    inviterCharacterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    inviterRealmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    seen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    note = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    expiration = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendship_invite", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendship_invite_character_inviteeCharacterId_inviteeRealmId",
                        columns: x => new { x.inviteeCharacterId, x.inviteeRealmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_friendship_invite_character_inviterCharacterId_inviterRealmId",
                        columns: x => new { x.inviterCharacterId, x.inviterRealmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "account_friend",
                columns: table => new
                {
                    accountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    friendAccountId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_friend", x => new { x.accountId, x.friendAccountId });
                    table.ForeignKey(
                        name: "FK_account_friend_account_accountId",
                        column: x => x.accountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_friend_friendship_account_friendAccountId",
                        column: x => x.friendAccountId,
                        principalTable: "friendship_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "account_friend_inverse",
                columns: table => new
                {
                    accountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    friendAccountId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_friend_inverse", x => new { x.accountId, x.friendAccountId });
                    table.ForeignKey(
                        name: "FK_account_friend_inverse_account_accountId",
                        column: x => x.accountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_friend_inverse_friendship_account_friendAccountId",
                        column: x => x.friendAccountId,
                        principalTable: "friendship_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "account_friend_invite",
                columns: table => new
                {
                    accountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    friendAccountInviteId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_friend_invite", x => new { x.accountId, x.friendAccountInviteId });
                    table.ForeignKey(
                        name: "FK_account_friend_invite_account_accountId",
                        column: x => x.accountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_friend_invite_friendship_account_invite_friendAccoun~",
                        column: x => x.friendAccountInviteId,
                        principalTable: "friendship_account_invite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "account_friend_invite_pending",
                columns: table => new
                {
                    accountId = table.Column<uint>(type: "int unsigned", nullable: false),
                    friendAccountInviteId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_friend_invite_pending", x => new { x.accountId, x.friendAccountInviteId });
                    table.ForeignKey(
                        name: "FK_account_friend_invite_pending_account_accountId",
                        column: x => x.accountId,
                        principalTable: "account",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_friend_invite_pending_friendship_account_invite_frie~",
                        column: x => x.friendAccountInviteId,
                        principalTable: "friendship_account_invite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character_friend",
                columns: table => new
                {
                    characterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    realmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    friendId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_friend", x => new { x.characterId, x.realmId, x.friendId });
                    table.ForeignKey(
                        name: "FK_character_friend_character_characterId_realmId",
                        columns: x => new { x.characterId, x.realmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_friend_friendship_friendId",
                        column: x => x.friendId,
                        principalTable: "friendship",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character_friend_inverse",
                columns: table => new
                {
                    characterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    realmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    friendId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_friend_inverse", x => new { x.characterId, x.realmId, x.friendId });
                    table.ForeignKey(
                        name: "FK_character_friend_inverse_character_characterId_realmId",
                        columns: x => new { x.characterId, x.realmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_friend_inverse_friendship_friendId",
                        column: x => x.friendId,
                        principalTable: "friendship",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character_friend_invite",
                columns: table => new
                {
                    characterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    realmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    friendInviteId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_friend_invite", x => new { x.characterId, x.realmId, x.friendInviteId });
                    table.ForeignKey(
                        name: "FK_character_friend_invite_character_characterId_realmId",
                        columns: x => new { x.characterId, x.realmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_friend_invite_friendship_invite_friendInviteId",
                        column: x => x.friendInviteId,
                        principalTable: "friendship_invite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "character_friend_invite_pending",
                columns: table => new
                {
                    characterId = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    realmId = table.Column<ushort>(type: "smallint unsigned", nullable: false),
                    friendInviteId = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_friend_invite_pending", x => new { x.characterId, x.realmId, x.friendInviteId });
                    table.ForeignKey(
                        name: "FK_character_friend_invite_pending_character_characterId_realmId",
                        columns: x => new { x.characterId, x.realmId },
                        principalTable: "character",
                        principalColumns: new[] { "characterId", "realmId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_character_friend_invite_pending_friendship_invite_friendInvi~",
                        column: x => x.friendInviteId,
                        principalTable: "friendship_invite",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_account_activeCharacterId_activeRealmId",
                table: "account",
                columns: new[] { "activeCharacterId", "activeRealmId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_email",
                table: "account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_nickname",
                table: "account",
                column: "nickname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_friend_accountId",
                table: "account_friend",
                column: "accountId");

            migrationBuilder.CreateIndex(
                name: "IX_account_friend_friendAccountId",
                table: "account_friend",
                column: "friendAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_friend_inverse_accountId",
                table: "account_friend_inverse",
                column: "accountId");

            migrationBuilder.CreateIndex(
                name: "IX_account_friend_inverse_friendAccountId",
                table: "account_friend_inverse",
                column: "friendAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_friend_invite_accountId",
                table: "account_friend_invite",
                column: "accountId");

            migrationBuilder.CreateIndex(
                name: "IX_account_friend_invite_friendAccountInviteId",
                table: "account_friend_invite",
                column: "friendAccountInviteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_friend_invite_pending_accountId",
                table: "account_friend_invite_pending",
                column: "accountId");

            migrationBuilder.CreateIndex(
                name: "IX_account_friend_invite_pending_friendAccountInviteId",
                table: "account_friend_invite_pending",
                column: "friendAccountInviteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_character_AccountId",
                table: "character",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_character_friend_characterId_realmId",
                table: "character_friend",
                columns: new[] { "characterId", "realmId" });

            migrationBuilder.CreateIndex(
                name: "IX_character_friend_friendId",
                table: "character_friend",
                column: "friendId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_character_friend_inverse_characterId_realmId",
                table: "character_friend_inverse",
                columns: new[] { "characterId", "realmId" });

            migrationBuilder.CreateIndex(
                name: "IX_character_friend_inverse_friendId",
                table: "character_friend_inverse",
                column: "friendId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_character_friend_invite_characterId_realmId",
                table: "character_friend_invite",
                columns: new[] { "characterId", "realmId" });

            migrationBuilder.CreateIndex(
                name: "IX_character_friend_invite_friendInviteId",
                table: "character_friend_invite",
                column: "friendInviteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_character_friend_invite_pending_characterId_realmId",
                table: "character_friend_invite_pending",
                columns: new[] { "characterId", "realmId" });

            migrationBuilder.CreateIndex(
                name: "IX_character_friend_invite_pending_friendInviteId",
                table: "character_friend_invite_pending",
                column: "friendInviteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_character_stat_characterId_realmId",
                table: "character_stat",
                columns: new[] { "characterId", "realmId" });

            migrationBuilder.CreateIndex(
                name: "IX_friendship_friendCharacterId_friendRealmId",
                table: "friendship",
                columns: new[] { "friendCharacterId", "friendRealmId" });

            migrationBuilder.CreateIndex(
                name: "IX_friendship_inviterCharacterId_inviterRealmId_friendCharacter~",
                table: "friendship",
                columns: new[] { "inviterCharacterId", "inviterRealmId", "friendCharacterId", "friendRealmId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_friendship_account_inviteeAccountId",
                table: "friendship_account",
                column: "inviteeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_friendship_account_inviterAccountId",
                table: "friendship_account",
                column: "inviterAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_friendship_account_invite_inviteeAccountId",
                table: "friendship_account_invite",
                column: "inviteeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_friendship_account_invite_inviterAccountId",
                table: "friendship_account_invite",
                column: "inviterAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_friendship_invite_inviteeCharacterId_inviteeRealmId",
                table: "friendship_invite",
                columns: new[] { "inviteeCharacterId", "inviteeRealmId" });

            migrationBuilder.CreateIndex(
                name: "IX_friendship_invite_inviterCharacterId_inviterRealmId",
                table: "friendship_invite",
                columns: new[] { "inviterCharacterId", "inviterRealmId" });

            migrationBuilder.AddForeignKey(
                name: "FK_account_character_activeCharacterId_activeRealmId",
                table: "account",
                columns: new[] { "activeCharacterId", "activeRealmId" },
                principalTable: "character",
                principalColumns: new[] { "characterId", "realmId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_character_activeCharacterId_activeRealmId",
                table: "account");

            migrationBuilder.DropTable(
                name: "account_friend");

            migrationBuilder.DropTable(
                name: "account_friend_inverse");

            migrationBuilder.DropTable(
                name: "account_friend_invite");

            migrationBuilder.DropTable(
                name: "account_friend_invite_pending");

            migrationBuilder.DropTable(
                name: "character_friend");

            migrationBuilder.DropTable(
                name: "character_friend_inverse");

            migrationBuilder.DropTable(
                name: "character_friend_invite");

            migrationBuilder.DropTable(
                name: "character_friend_invite_pending");

            migrationBuilder.DropTable(
                name: "character_stat");

            migrationBuilder.DropTable(
                name: "InternalMessage");

            migrationBuilder.DropTable(
                name: "friendship_account");

            migrationBuilder.DropTable(
                name: "friendship_account_invite");

            migrationBuilder.DropTable(
                name: "friendship");

            migrationBuilder.DropTable(
                name: "friendship_invite");

            migrationBuilder.DropTable(
                name: "character");

            migrationBuilder.DropTable(
                name: "account");
        }
    }
}
