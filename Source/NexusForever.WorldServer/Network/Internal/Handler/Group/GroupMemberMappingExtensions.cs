using NexusForever.Network.World.Message.Model;
using InternalGroupMember = NexusForever.Network.Internal.Message.Group.Shared.GroupMember;
using NetworkGroupMember = NexusForever.Network.World.Message.Model.Shared.GroupMember;

namespace NexusForever.WorldServer.Network.Internal.Handler.Group
{
    public static class GroupMemberMappingExtensions
    {
        public static NetworkGroupMember ToNetworkGroupMember(this InternalGroupMember member)
        {
            return new NetworkGroupMember
            {
                MemberIdentity = member.Identity.ToNetworkIdentity(),
                Flags          = member.Flags,
                Member         = member.Character.ToNetworkGroupCharacter(),
                GroupIndex     = member.GroupIndex
            };
        }

        public static ServerGroupMemberStatUpdate ToNetworkGroupMemberStatUpdate(this InternalGroupMember member, ulong groupId)
        {
            return new ServerGroupMemberStatUpdate
            {
                GroupId          = groupId,
                TargetPlayer     = member.Identity.ToNetworkIdentity(),
                Level            = member.Character.Level,
                EffectiveLevel   = member.Character.EffectiveLevel,
                Health           = member.Character.Health,
                HealthMax        = member.Character.HealthMax,
                Shield           = member.Character.Shield,
                ShieldMax        = member.Character.ShieldMax,
                Absorption       = member.Character.Absorption,
                AbsorptionMax    = member.Character.AbsorptionMax,
                Mana             = member.Character.Focus,
                ManaMax          = member.Character.FocusMax,
                HealingAbsorb    = member.Character.HealingAbsorb,
                HealingAbsorbMax = member.Character.HealingAbsorbMax,
                PhaseFlags1      = member.Character.PhaseFlags1,
                PhaseFlags2      = member.Character.PhaseFlags2,
                Path             = member.Character.Path
            };
        }
    }
}
