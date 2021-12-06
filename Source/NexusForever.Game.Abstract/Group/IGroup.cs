using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Group
{
    public interface IGroup : IUpdate, INetworkBuildable<GroupInfo>
    {
        GroupFlags Flags { get; set; }
        ulong Id { get; }
        bool IsFull { get; }
        bool IsOpenWorld { get; }
        bool IsRaid { get; }
        IGroupMember Leader { get; set; }
        uint MaxGroupSize { get; set; }
        int MemberCount { get; }

        void AcceptInvite(IGroupInvite invite);
        void AcceptInvite(string inviteeName);
        void AddMember(IGroupMember addedMember);
        void BroadcastPacket(IWritable message);
        List<GroupMember> BuildGroupMembers();
        List<GroupMemberInfo> BuildMembersInfo();
        bool CanJoinGroup(out GroupResult result);
        void ConvertToRaid();
        IGroupInvite CreateInvite(IGroupMember inviter, IPlayer invitedPlayer, GroupInviteType type);
        IGroupMember CreateMember(IPlayer player);
        void DeclineInvite(IGroupInvite invite);
        void DeclineInvite(string inviteeName);
        void Disband();
        void ExpireInvite(IGroupInvite invite);
        IGroupMember FindMember(TargetPlayerIdentity target);
        uint GetMemberIndex(IGroupMember groupMember);
        void HandleJoinRequest(IPlayer prospective);
        void Invite(IPlayer inviter, IPlayer invitedPlayer);
        void KickMember(TargetPlayerIdentity target);
        void MarkUnit(uint unitId, GroupMarker marker);
        ulong NextInviteId();
        ulong NextMemberId();
        void PerformReadyCheck(IPlayer invoker, string message);
        void PrepareForReadyCheck();
        void Promote(TargetPlayerIdentity newLeader);
        void ReferMember(IGroupMember inviter, IPlayer invitee);
        void RemoveMember(IGroupMember memberToRemove);
        void SetGroupFlags(GroupFlags newFlags);
        void UpdateLootRules(LootRule lootRulesUnderThreshold, LootRule lootRulesThresholdAndOver, LootThreshold lootThreshold, HarvestLootRule harvestLootRule);
        void UpdateMemberRole(IGroupMember updater, TargetPlayerIdentity target, GroupMemberInfoFlags changedFlag, bool addPermission);
    }
}