using NexusForever.Game.Abstract.Group;
using NexusForever.Game.Static.Group;

namespace NexusForever.Game.Group
{
    public class GroupInvite : IGroupInvite
    {
        public const double InviteTimeout = 30d;

        public ulong InviteId { get; }
        public IGroup Group { get; }
        public ulong InvitedCharacterId { get; }
        public string InvitedCharacterName { get; }
        public IGroupMember Inviter { get; }
        public GroupInviteType Type { get; }
        public double ExpirationTime { get; set; } = InviteTimeout;

        /// <summary>
        /// Creates an instance of <see cref="GroupInvite"/>
        /// </summary>
        public GroupInvite(ulong id, IGroup group, ulong invitedCharacterId, string invitedCharacterName, IGroupMember inviter, GroupInviteType type)
        {
            InviteId = id;
            Group = group;
            InvitedCharacterId = invitedCharacterId;
            InvitedCharacterName = invitedCharacterName;
            Inviter = inviter;
            Type = type;
        }
    }
}
