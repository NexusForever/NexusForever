using NexusForever.WorldServer.Game.CharacterCache;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Game.TextFilter;
using NexusForever.WorldServer.Game.TextFilter.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System.Linq;

namespace NexusForever.WorldServer.Game.Guild
{
    public partial class GuildBase
    {
        [GuildOperationHandler(GuildOperation.Disband)]
        private GuildResultInfo GuildOperationDisband(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildResultInfo GetResult()
            {
                if (member.Rank.Index != 0)
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                return CanDisbandGuild();
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
                DisbandGuild();

            return info;
        }

        [GuildOperationHandler(GuildOperation.EditPlayerNote)]
        private GuildResultInfo GuildOperationEditPlayerNote(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildResultInfo GetResult()
            {
                if (!TextFilterManager.Instance.IsTextValid(operation.TextValue)
                    || !TextFilterManager.Instance.IsTextValid(operation.TextValue, UserText.GuildMemberNote))
                    return new GuildResultInfo(GuildResult.InvalidMemberNote);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                member.Note = operation.TextValue;
                AnnounceGuildMemberChange(member);
            }

            return info;
        }        

        [GuildOperationHandler(GuildOperation.InitGuildWindow)]
        private void GuildOperationInitGuildWindow(GuildMember member, Player player, ClientGuildOperation operation)
        {
            // Probably want to send roster update
        }

        [GuildOperationHandler(GuildOperation.MemberDemote)]
        private GuildResultInfo GuildOperationMemberDemote(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildMember targetMember = null;
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.ChangeMemberRank))
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                targetMember = GetMember(operation.TextValue);
                if (targetMember == null)
                    return new GuildResultInfo(GuildResult.CharacterNotInYourGuild, referenceString: operation.TextValue);

                if (member.Rank.Index >= targetMember.Rank.Index)
                    return new GuildResultInfo(GuildResult.CannotDemote);

                if (targetMember.Rank.Index == 9)
                    return new GuildResultInfo(GuildResult.CannotDemoteLowestRank);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                GuildRank newRank = GetDemotedRank(targetMember.Rank.Index);
                MemberChangeRank(targetMember, newRank);

                AnnounceGuildMemberChange(targetMember);
                AnnounceGuildResult(GuildResult.DemotedMember, referenceText: operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.MemberPromote)]
        private GuildResultInfo GuildOperationMemberPromote(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildMember targetMember = null;
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.ChangeMemberRank))
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                targetMember = GetMember(operation.TextValue);
                if (targetMember == null)
                    return new GuildResultInfo(GuildResult.CharacterNotInYourGuild, referenceString: operation.TextValue);

                if (member.Rank.Index >= targetMember.Rank.Index)
                    return new GuildResultInfo(GuildResult.CannotPromote);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                GuildRank newRank = GetPromotedRank(targetMember.Rank.Index);
                MemberChangeRank(targetMember, newRank);

                AnnounceGuildMemberChange(targetMember);
                AnnounceGuildResult(GuildResult.PromotedMember, referenceText: operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.MemberInvite)]
        private GuildResultInfo GuildOperationMemberInvite(GuildMember member, Player player, ClientGuildOperation operation)
        {
            Player target = null;
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.Invite))
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                target = CharacterManager.Instance.GetPlayer(operation.TextValue);
                if (target == null)
                    return new GuildResultInfo(GuildResult.UnknownCharacter, referenceString: operation.TextValue);

                return target.GuildManager.CanInviteToGuild(Id);
            }

            GuildResultInfo info = GetResult();
            if (info.Result != GuildResult.Success)
                return info;

            target.GuildManager.InviteToGuild(Id, player);
            return new GuildResultInfo(GuildResult.InviteSent, referenceString: operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.MemberRemove)]
        private GuildResultInfo GuildOperationMemberRemove(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildMember targetMember = null;
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.Kick))
                    return new GuildResultInfo(GuildResult.RankLacksRankRenamePermission);

                targetMember = GetMember(operation.TextValue);
                if (targetMember == null)
                    return new GuildResultInfo(GuildResult.CharacterNotInYourGuild, referenceString: operation.TextValue);

                if (member.Rank.Index >= targetMember.Rank.Index)
                    return new GuildResultInfo(GuildResult.CannotKickHigherOrEqualRankedMember);

                return CanLeaveGuild(targetMember);
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                // if the player is online handle through the local manager otherwise directly in the guild
                ICharacter targetCharacter = CharacterManager.Instance.GetCharacterInfo(targetMember.CharacterId);
                if (targetCharacter is Player targetPlayer)
                    targetPlayer.GuildManager.LeaveGuild(Id, GuildResult.KickedYou);
                else
                    LeaveGuild(targetMember);

                AnnounceGuildResult(GuildResult.KickedMember, referenceText: targetCharacter.Name);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.MemberQuit)]
        private GuildResultInfo GuildOperationMemberQuit(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildResultInfo info = CanLeaveGuild(member);
            if (info.Result == GuildResult.Success)
            {
                player.GuildManager.LeaveGuild(Id, GuildResult.YouQuit);
                AnnounceGuildResult(GuildResult.MemberQuit, referenceText: player.Name);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RankAdd)]
        private GuildResultInfo GuildOperationRankAdd(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.CreateAndRemoveRank))
                    return new GuildResultInfo(GuildResult.RankLacksRankRenamePermission);

                if (GetRank((byte)operation.Rank) != null)
                    return new GuildResultInfo(GuildResult.InvalidRank, operation.Rank, operation.TextValue);

                if (RankExists(operation.TextValue))
                    return new GuildResultInfo(GuildResult.DuplicateRankName, referenceString: operation.TextValue);

                if (!TextFilterManager.Instance.IsTextValid(operation.TextValue)
                    || !TextFilterManager.Instance.IsTextValid(operation.TextValue, UserText.GuildRankName))
                    return new GuildResultInfo(GuildResult.InvalidRankName, referenceString: operation.TextValue);

                // TODO: check if mask options are valid for this guild type

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                AddRank((byte)operation.Rank, operation.TextValue, (GuildRankPermission)operation.Data);
                AnnounceGuildRankChange();
                AnnounceGuildResult(GuildResult.RankCreated, operation.Rank, operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RankDelete)]
        private GuildResultInfo GuildOperationRankDelete(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildRank rank = null;
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.CreateAndRemoveRank))
                    return new GuildResultInfo(GuildResult.RankLacksRankRenamePermission);

                rank = GetRank((byte)operation.Rank);
                if (rank == null)
                    return new GuildResultInfo(GuildResult.InvalidRank, operation.Rank, operation.TextValue);

                if (member.Rank.Index >= operation.Rank)
                    return new GuildResultInfo(GuildResult.CanOnlyModifyLowerRanks);

                if (operation.Rank < 2 || operation.Rank > 8)
                    return new GuildResultInfo(GuildResult.CannotDeleteDefaultRanks, referenceId: operation.Rank);

                if (rank.Any())
                    return new GuildResultInfo(GuildResult.CanOnlyDeleteEmptyRanks);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                RemoveRank((byte)operation.Rank);
                AnnounceGuildRankChange();
                AnnounceGuildResult(GuildResult.RankDeleted, referenceText: operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RankRename)]
        private GuildResultInfo GuildOperationRankRename(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildRank rank = null;
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.RenameRank))
                    return new GuildResultInfo(GuildResult.RankLacksRankRenamePermission);

                rank = GetRank((byte)operation.Rank);
                if (rank == null)
                    return new GuildResultInfo(GuildResult.InvalidRank, operation.Rank, operation.TextValue);

                if (member.Rank.Index >= operation.Rank)
                    return new GuildResultInfo(GuildResult.CanOnlyModifyLowerRanks);

                if (RankExists(operation.TextValue))
                    return new GuildResultInfo(GuildResult.DuplicateRankName, referenceString: operation.TextValue);

                if (!TextFilterManager.Instance.IsTextValid(operation.TextValue)
                    || !TextFilterManager.Instance.IsTextValid(operation.TextValue, UserText.GuildRankName))
                    return new GuildResultInfo(GuildResult.InvalidRankName, referenceString: operation.TextValue);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                rank.Name = operation.TextValue;
                AnnounceGuildRankChange();
                AnnounceGuildResult(GuildResult.RankRenamed, operation.Rank, operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RankPermissions)]
        private GuildResultInfo GuildOperationRankPermissions(GuildMember member, Player player, ClientGuildOperation operation)
        {
            GuildRank rank = null;
            GuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.EditLowerRankPermissions))
                    return new GuildResultInfo(GuildResult.RankLacksRankRenamePermission);

                rank = GetRank((byte)operation.Rank);
                if (rank == null)
                    return new GuildResultInfo(GuildResult.InvalidRank, operation.Rank, operation.TextValue);
                
                // TODO: check if mask options are valid for this guild type

                if (member.Rank.Index >= operation.Rank)
                    return new GuildResultInfo(GuildResult.CanOnlyModifyLowerRanks);

                return new GuildResultInfo(GuildResult.Success);
            }

            GuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                // TODO: research why disabled needs to be removed
                var permissions = (GuildRankPermission)operation.Data & ~GuildRankPermission.Disabled;
                rank.Permissions = permissions;

                AnnounceGuildRankChange();
                AnnounceGuildResult(GuildResult.RankModified, operation.Rank, operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RosterRequest)]
        private void GuildOperationRosterRequest(GuildMember member, Player player, ClientGuildOperation operation)
        {
            SendGuildRoster(player.Session);
        }

        [GuildOperationHandler(GuildOperation.SetNameplateAffiliation)]
        private void GuildOperationSetNameplateAffiliation(GuildMember member, Player player, ClientGuildOperation operation)
        {
            player.GuildManager.UpdateGuildAffiliation(Id);
        }
    }
}
