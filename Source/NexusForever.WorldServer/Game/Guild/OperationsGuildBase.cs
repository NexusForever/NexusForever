using NexusForever.Shared;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NexusForever.WorldServer.Game.Guild
{
    public sealed partial class GlobalGuildManager : Singleton<GlobalGuildManager>
    {
        [GuildOperationHandler(GuildOperation.Disband)]
        private void GuildOperationDisband(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;

            GuildResult GetResult()
            {
                if (memberRank.Index > 0)
                    return GuildResult.RankLacksSufficientPermissions;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                foreach(ulong characterId in guildBase.OnlineMembers)
                {
                    guildBase.RemoveMember(characterId, out WorldSession memberSession);
                    HandlePlayerRemove(memberSession, GuildResult.GuildDisbanded, guildBase, referenceText: guildBase.Name);
                }

                DeleteGuild(guildBase.Id);
            }
            else
                SendGuildResult(session, result, guildBase);
        }

        [GuildOperationHandler(GuildOperation.EditPlayerNote)]
        private void GuildOperationEditPlayerNote(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var member = guildBase.GetMember(session.Player.CharacterId);

            GuildResult GetResult()
            {
                // TODO: Set GuildResult.InvalidMemberNote when rules for note fail. What rules?

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                member.SetNote(operation.TextValue);
                guildBase.AnnounceGuildMemberChange(session.Player.CharacterId);
            }
            else
                SendGuildResult(session, result, guildBase, referenceText: operation.TextValue);
        }
        

        [GuildOperationHandler(GuildOperation.InitGuildWindow)]
        private void GuildOperationInitGuildWindow(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            // Probably want to send roster update
        }

        [GuildOperationHandler(GuildOperation.MemberDemote)]
        private void GuildOperationMemberDemote(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;
            var targetMember = guildBase.GetMember(operation.TextValue);

            GuildResult GetResult()
            {
                if ((memberRank.GuildPermission & GuildRankPermission.ChangeMemberRank) == 0)
                    return GuildResult.RankLacksSufficientPermissions;

                if (targetMember == null)
                    return GuildResult.CharacterNotInYourGuild;

                if (memberRank.Index >= targetMember.Rank.Index)
                    return GuildResult.CanOnlyDemoteLowerRankedMembers;

                if (memberRank.Index == 9)
                    return GuildResult.MemberIsAlreadyLowestRank;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                Rank newRank = guildBase.GetDemotedRank(targetMember.Rank.Index);
                targetMember.ChangeRank(newRank);
                guildBase.AnnounceGuildMemberChange(targetMember.CharacterId);
                guildBase.AnnounceGuildResult(GuildResult.DemotedMember, referenceText: operation.TextValue);
            }
            else
                SendGuildResult(session, result, guildBase, referenceText: operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.MemberPromote)]
        private void GuildOperationMemberPromote(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;
            var targetMember = guildBase.GetMember(operation.TextValue);

            GuildResult GetResult()
            {
                if ((memberRank.GuildPermission & GuildRankPermission.ChangeMemberRank) == 0)
                    return GuildResult.RankLacksSufficientPermissions;

                if (targetMember == null)
                    return GuildResult.CharacterNotInYourGuild;

                if (memberRank.Index >= targetMember.Rank.Index)
                    return GuildResult.CannotPromoteMemberAboveYourRank;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                Rank newRank = guildBase.GetPromotedRank(targetMember.Rank.Index);
                targetMember.ChangeRank(newRank);
                guildBase.AnnounceGuildMemberChange(targetMember.CharacterId);
                guildBase.AnnounceGuildResult(GuildResult.PromotedMember, referenceText: operation.TextValue);
            }
            else
                SendGuildResult(session, result, guildBase, referenceText: operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.MemberInvite)]
        private void GuildOperationMemberInvite(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;
            var targetCharacter = CharacterManager.Instance.GetCharacterInfo(operation.TextValue);

            GuildResult GetResult()
            {
                if ((memberRank.GuildPermission & GuildRankPermission.Invite) == 0)
                    return GuildResult.RankLacksSufficientPermissions;

                if (!CharacterManager.Instance.IsCharacter(operation.TextValue))
                    return GuildResult.UnknownCharacter;

                if (guildBase.GetMemberCount() >= maxGuildSize[guildBase.Type])
                    return GuildResult.CannotInviteGuildFull;

                return GuildResult.Success;
            }
            
            GuildResult result = GetResult();
            if (result != GuildResult.Success)
            {
                SendGuildResult(session, result, guildBase, referenceText: operation.TextValue);
                return;
            }

            var targetSession = NetworkManager<WorldSession>.Instance.GetSession(i => i.Player?.CharacterId == targetCharacter.CharacterId);
            GuildResult GetCharacterResult()
            {
                if (targetSession == null)
                    return GuildResult.UnknownCharacter;

                if (targetSession.Player.PendingGuildInvite != null)
                    return GuildResult.CharacterAlreadyHasAGuildInvite;

                if (guildBase.Type == GuildType.Guild && targetSession.Player.GuildId > 0)
                    return GuildResult.CharacterCannotJoinMoreGuilds;

                return GuildResult.Success;
            }

            result = GetCharacterResult();
            if (result == GuildResult.Success)
            {
                targetSession.Player.PendingGuildInvite = new GuildInvite
                {
                    GuildId = guildBase.Id,
                    InviteeId = session.Player.CharacterId
                };
                SendPendingInvite(targetSession);
                SendGuildResult(session, GuildResult.CharacterInvited, guildBase, referenceText: targetCharacter.Name);
            }
            else
                SendGuildResult(session, result, guildBase, referenceText: targetCharacter != null ? targetCharacter.Name : operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.MemberRemove)]
        private void GuildOperationMemberRemove(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;
            var targetMember = guildBase.GetMember(operation.TextValue);

            GuildResult GetResult()
            {
                if ((memberRank.GuildPermission & GuildRankPermission.Kick) == 0)
                    return GuildResult.RankLacksSufficientPermissions;

                if (targetMember == null)
                    return GuildResult.CharacterNotInYourGuild;

                if (memberRank.Index >= targetMember.Rank.Index)
                    return GuildResult.CannotKickHigherOrEqualRankedMember;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                guildBase.RemoveMember(targetMember.CharacterId, out WorldSession memberSession);
                
                // Let player know they have been removed and update necessary values
                if (memberSession != null)
                    HandlePlayerRemove(memberSession, GuildResult.KickedYou, guildBase);

                // Announce to guild that player has been removed
                guildBase.SendToOnlineUsers(new ServerGuildMemberRemove
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = guildBase.Id,
                    PlayerIdentity = new TargetPlayerIdentity
                    {
                        RealmId = WorldServer.RealmId,
                        CharacterId = targetMember.CharacterId
                    },
                });
                guildBase.AnnounceGuildResult(GuildResult.KickedMember, referenceText: CharacterManager.Instance.GetCharacterInfo(targetMember.CharacterId).Name);
            }
            else
                SendGuildResult(session, result, guildBase, referenceText: operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.MemberQuit)]
        private void GuildOperationMemberQuit(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;
            
            GuildResult GetResult()
            {
                if (memberRank.Index == 0)
                    return GuildResult.GuildmasterCannotLeaveGuild;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                guildBase.RemoveMember(session.Player.CharacterId, out WorldSession memberSession);

                HandlePlayerRemove(session, GuildResult.YouQuit, guildBase, guildBase.Name);

                // Notify guild members of player quitting
                guildBase.SendToOnlineUsers(new ServerGuildMemberRemove
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = guildBase.Id,
                    PlayerIdentity = new TargetPlayerIdentity
                    {
                        RealmId = WorldServer.RealmId,
                        CharacterId = session.Player.CharacterId
                    },
                });
                guildBase.AnnounceGuildResult(GuildResult.MemberQuit, referenceText: session.Player.Name);
            }
        }

        [GuildOperationHandler(GuildOperation.RankAdd)]
        private void GuildOperationRankAdd(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;

            GuildResult GetResult()
            {
                if ((memberRank.GuildPermission & GuildRankPermission.CreateAndRemoveRank) == 0)
                    return GuildResult.RankLacksSufficientPermissions;

                if (guildBase.GetRank((byte)operation.Rank) != null)
                    return GuildResult.InvalidRank;

                if (guildBase.RankExists(operation.TextValue))
                    return GuildResult.DuplicateRankName;

                if (Regex.IsMatch(operation.TextValue, @"[^A-Za-z0-9\s]")) // Ensure only Alphanumeric characters are used
                    return GuildResult.InvalidRankName;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                guildBase.AddRank(new Rank(operation.TextValue, guildBase.Id, (byte)operation.Rank, (GuildRankPermission)operation.Data, 0, 0, 0));
                guildBase.SendToOnlineUsers(new ServerGuildRankChange
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = guildBase.Id,
                    Ranks = guildBase.GetGuildRanksPackets().ToList()
                });
                guildBase.AnnounceGuildResult(GuildResult.RankCreated, operation.Rank, operation.TextValue);
            }
            else
                SendGuildResult(session, result, guildBase, operation.Rank, operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.RankDelete)]
        private void GuildOperationRankDelete(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;

            GuildResult GetResult()
            {
                if ((memberRank.GuildPermission & GuildRankPermission.CreateAndRemoveRank) == 0)
                    return GuildResult.RankLacksSufficientPermissions;

                if (guildBase.GetRank((byte)operation.Rank) == null)
                    return GuildResult.InvalidRank;

                if (memberRank.Index >= operation.Rank)
                    return GuildResult.CanOnlyModifyRanksBelowYours;

                if (operation.Rank < 2 || operation.Rank > 8)
                    return GuildResult.CannotDeleteDefaultRanks;

                if (guildBase.GetMembersOfRank((byte)operation.Rank).Count() > 0)
                    return GuildResult.CanOnlyDeleteEmptyRanks;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                string rankName = guildBase.GetRank((byte)operation.Rank).Name;
                guildBase.RemoveRank((byte)operation.Rank);
                guildBase.AnnounceGuildResult(GuildResult.RankDeleted, operation.Rank, rankName);
                guildBase.SendToOnlineUsers(new ServerGuildRankChange
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = guildBase.Id,
                    Ranks = guildBase.GetGuildRanksPackets().ToList()
                });
            }
            else
                SendGuildResult(session, result, guildBase, operation.Rank, operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.RankRename)]
        private void GuildOperationRankRename(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            var memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;

            GuildResult GetResult()
            {
                if ((memberRank.GuildPermission & GuildRankPermission.RenameRank) == 0)
                    return GuildResult.RankLacksRankRenamePermission;

                if (guildBase.GetRank((byte)operation.Rank) == null)
                    return GuildResult.InvalidRank;

                if (memberRank.Index >= operation.Rank)
                    return GuildResult.CanOnlyModifyRanksBelowYours;

                if (guildBase.RankExists(operation.TextValue))
                    return GuildResult.DuplicateRankName;

                if (Regex.IsMatch(operation.TextValue, @"[^A-Za-z0-9\s]")) // Ensure only Alphanumeric characters are used
                    return GuildResult.InvalidRankName;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                guildBase.GetRank((byte)operation.Rank).Rename(operation.TextValue);
                guildBase.SendToOnlineUsers(new ServerGuildRankChange
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = guildBase.Id,
                    Ranks = guildBase.GetGuildRanksPackets().ToList()
                });
                guildBase.AnnounceGuildResult(GuildResult.RankRenamed, operation.Rank, operation.TextValue);
            }
            else
                SendGuildResult(session, result, guildBase, operation.Rank, operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.RankPermissions)]
        private void GuildOperationRankPermissions(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            Rank rankToModify = guildBase.GetRank((byte)operation.Rank);
            Rank memberRank = guildBase.GetMember(session.Player.CharacterId).Rank;

            GuildResult GetResult()
            {
                if ((memberRank.GuildPermission & GuildRankPermission.EditLowerRankPermissions) == 0)
                    return GuildResult.RankLacksRankRenamePermission;

                if (rankToModify == null)
                    return GuildResult.InvalidRank;

                if (memberRank.Index >= operation.Rank)
                    return GuildResult.CanOnlyModifyRanksBelowYours;

                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                ulong newPermissionMask = operation.Data;
                if (newPermissionMask % 2 != 0)
                    newPermissionMask -= 1;

                rankToModify.SetPermission((GuildRankPermission)newPermissionMask);
                guildBase.SendToOnlineUsers(new ServerGuildRankChange
                {
                    RealmId = WorldServer.RealmId,
                    GuildId = guildBase.Id,
                    Ranks = guildBase.GetGuildRanksPackets().ToList()
                });
                guildBase.AnnounceGuildResult(GuildResult.RankModified, operation.Rank, operation.TextValue);
            }
            else
                SendGuildResult(session, result, guildBase);
        }

        [GuildOperationHandler(GuildOperation.RosterRequest)]
        private void GuildOperationRosterRequest(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            foreach (ulong guildId in session.Player.GuildMemberships)
            {
                guilds.TryGetValue(guildId, out GuildBase guild);
                if (guild != null)
                {
                    if (guild.GetMember(session.Player.CharacterId) != null)
                        SendGuildRoster(session, guilds[guildId].GetGuildMembersPackets().ToList(), guildId);
                }
            }

            SendPendingInvite(session);
        }

        [GuildOperationHandler(GuildOperation.SetNameplateAffiliation)]
        private void GuildOperationSetNameplateAffiliation(WorldSession session, ClientGuildOperation operation, GuildBase guildBase)
        {
            GuildResult GetResult()
            {
                return GuildResult.Success;
            }

            GuildResult result = GetResult();
            if (result == GuildResult.Success)
            {
                session.Player.GuildAffiliation = guildBase.Id;
                SendGuildAffiliation(session);
            }
            else
                SendGuildResult(session, result, guildBase);
        }
    }
}
