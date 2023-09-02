using NexusForever.Game.Static.Group;

namespace NexusForever.Game.Abstract.Group
{
    public interface IGroupInvite
    {
        double ExpirationTime { get; set; }
        IGroup Group { get; }
        ulong InvitedCharacterId { get; }
        string InvitedCharacterName { get; }
        ulong InviteId { get; }
        IGroupMember Inviter { get; }
        GroupInviteType Type { get; }
    }
}