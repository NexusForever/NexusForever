using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingRoleEnforcerResultMember
    {
        ulong CharacterId { get; init; }
        Role Role { get; set; }
    }
}
