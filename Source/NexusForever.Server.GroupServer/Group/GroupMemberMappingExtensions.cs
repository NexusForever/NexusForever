using NexusForever.Server.GroupServer.Character;
using InternalGroupMember = NexusForever.Network.Internal.Message.Group.Shared.GroupMember;

namespace NexusForever.Server.GroupServer.Group
{
    public static class GroupMemberMappingExtensions
    {
        public static async Task<InternalGroupMember> ToInternalGroupMember(this GroupMember member)
        {
            return new InternalGroupMember
            {
                Identity   = member.Identity.ToInternalIdentity(),
                GroupIndex = member.Index,
                Flags      = member.Flags,
                Character  = (await member.GetCharacterAsync()).ToGroupCharacter(),
            };
        }
    }
}
