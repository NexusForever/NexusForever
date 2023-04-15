using NexusForever.Game.Abstract.Character;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Character;
using NexusForever.Game.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.Game.Static.TextFilter;
using NexusForever.Game.Text.Filter;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Guild
{
    public partial class GuildBase
    {
        [GuildOperationHandler(GuildOperation.Disband)]
        private IGuildResultInfo GuildOperationDisband(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildResultInfo GetResult()
            {
                if (member.Rank.Index != 0)
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                return CanDisbandGuild();
            }

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
                DisbandGuild();

            return info;
        }

        [GuildOperationHandler(GuildOperation.EditPlayerNote)]
        private IGuildResultInfo GuildOperationEditPlayerNote(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildResultInfo GetResult()
            {
                if (!TextFilterManager.Instance.IsTextValid(operation.TextValue)
                    || !TextFilterManager.Instance.IsTextValid(operation.TextValue, UserText.GuildMemberNote))
                    return new GuildResultInfo(GuildResult.InvalidMemberNote);

                return new GuildResultInfo(GuildResult.Success);
            }

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                member.Note = operation.TextValue;
                AnnounceGuildMemberChange(member);
            }

            return info;
        }        

        [GuildOperationHandler(GuildOperation.InitGuildWindow)]
        private void GuildOperationInitGuildWindow(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            // Probably want to send roster update
        }

        [GuildOperationHandler(GuildOperation.MemberDemote)]
        private IGuildResultInfo GuildOperationMemberDemote(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildMember targetMember = null;
            IGuildResultInfo GetResult()
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

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                IGuildRank newRank = GetDemotedRank(targetMember.Rank.Index);
                MemberChangeRank(targetMember, newRank);

                AnnounceGuildMemberChange(targetMember);
                AnnounceGuildResult(GuildResult.DemotedMember, referenceText: operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.MemberPromote)]
        private IGuildResultInfo GuildOperationMemberPromote(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildMember targetMember = null;
            IGuildResultInfo GetResult()
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

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                IGuildRank newRank = GetPromotedRank(targetMember.Rank.Index);
                MemberChangeRank(targetMember, newRank);

                AnnounceGuildMemberChange(targetMember);
                AnnounceGuildResult(GuildResult.PromotedMember, referenceText: operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.MemberInvite)]
        private IGuildResultInfo GuildOperationMemberInvite(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IPlayer target = null;
            IGuildResultInfo GetResult()
            {
                if (!member.Rank.HasPermission(GuildRankPermission.Invite))
                    return new GuildResultInfo(GuildResult.RankLacksSufficientPermissions);

                target = PlayerManager.Instance.GetPlayer(operation.TextValue);
                if (target == null)
                    return new GuildResultInfo(GuildResult.UnknownCharacter, referenceString: operation.TextValue);

                return target.GuildManager.CanInviteToGuild(Id);
            }

            IGuildResultInfo info = GetResult();
            if (info.Result != GuildResult.Success)
                return info;

            target.GuildManager.InviteToGuild(Id, player);
            return new GuildResultInfo(GuildResult.InviteSent, referenceString: operation.TextValue);
        }

        [GuildOperationHandler(GuildOperation.MemberRemove)]
        private IGuildResultInfo GuildOperationMemberRemove(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildMember targetMember = null;
            IGuildResultInfo GetResult()
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

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                // if the player is online handle through the local manager otherwise directly in the guild
                IPlayer targetPlayer = PlayerManager.Instance.GetPlayer(targetMember.CharacterId);
                if (targetPlayer != null)
                    targetPlayer.GuildManager.LeaveGuild(Id, GuildResult.KickedYou);
                else
                    LeaveGuild(targetMember);

                AnnounceGuildResult(GuildResult.KickedMember, referenceText: targetPlayer.Name);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.MemberQuit)]
        private IGuildResultInfo GuildOperationMemberQuit(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildResultInfo info = CanLeaveGuild(member);
            if (info.Result == GuildResult.Success)
            {
                player.GuildManager.LeaveGuild(Id, GuildResult.YouQuit);
                AnnounceGuildResult(GuildResult.MemberQuit, referenceText: player.Name);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RankAdd)]
        private IGuildResultInfo GuildOperationRankAdd(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildResultInfo GetResult()
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

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                AddRank((byte)operation.Rank, operation.TextValue, (GuildRankPermission)operation.Data.UInt32Data);
                AnnounceGuildRankChange();
                AnnounceGuildResult(GuildResult.RankCreated, operation.Rank, operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RankDelete)]
        private IGuildResultInfo GuildOperationRankDelete(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildRank rank = null;
            IGuildResultInfo GetResult()
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

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                RemoveRank((byte)operation.Rank);
                AnnounceGuildRankChange();
                AnnounceGuildResult(GuildResult.RankDeleted, referenceText: operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RankRename)]
        private IGuildResultInfo GuildOperationRankRename(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildRank rank = null;
            IGuildResultInfo GetResult()
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

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                rank.Name = operation.TextValue;
                AnnounceGuildRankChange();
                AnnounceGuildResult(GuildResult.RankRenamed, operation.Rank, operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RankPermissions)]
        private IGuildResultInfo GuildOperationRankPermissions(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            IGuildRank rank = null;
            IGuildResultInfo GetResult()
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

            IGuildResultInfo info = GetResult();
            if (info.Result == GuildResult.Success)
            {
                // TODO: research why disabled needs to be removed
                var permissions = (GuildRankPermission)operation.Data.UInt32Data & ~GuildRankPermission.Disabled;
                rank.Permissions = permissions;

                AnnounceGuildRankChange();
                AnnounceGuildResult(GuildResult.RankModified, operation.Rank, operation.TextValue);
            }

            return info;
        }

        [GuildOperationHandler(GuildOperation.RosterRequest)]
        private void GuildOperationRosterRequest(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            SendGuildRoster(player.Session);
        }

        [GuildOperationHandler(GuildOperation.SetNameplateAffiliation)]
        private void GuildOperationSetNameplateAffiliation(IGuildMember member, IPlayer player, ClientGuildOperation operation)
        {
            player.GuildManager.UpdateGuildAffiliation(Id);
        }
    }
}
