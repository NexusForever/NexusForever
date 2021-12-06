using NexusForever.Game.Static.Group;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Group
{
    public interface IGroupMember : INetworkBuildable<GroupMember>
    {
        bool CanInvite { get; }
        bool CanKick { get; }
        bool CanMark { get; }
        bool CanReadyCheck { get; }
        ulong CharacterId { get; set; }
        GroupMemberInfoFlags Flags { get; set; }
        IGroup Group { get; }
        uint GroupIndex { get; }
        ulong Id { get; }
        bool IsPartyLeader { get; }
        ushort ZoneId { get; set; }

        ServerEntityGroupAssociation BuildGroupAssociation();
        ServerGroupMemberStatUpdate BuildGroupStatUpdate();
        GroupMemberInfo BuildMemberInfo();
        bool CanUpdateFlags(GroupMemberInfoFlags updateFlags, IGroupMember other);
        void PrepareForReadyCheck();
        void SetFlags(GroupMemberInfoFlags flags, bool value);
    }
}