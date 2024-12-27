using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingRoleEnforcerResultMember : IMatchingRoleEnforcerResultMember
    {
        public required ulong CharacterId { get; init; }
        public required Role Role { get; set; }
    }
}
