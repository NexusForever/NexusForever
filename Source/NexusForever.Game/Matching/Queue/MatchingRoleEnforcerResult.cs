using NexusForever.Game.Abstract.Matching.Queue;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingRoleEnforcerResult : IMatchingRoleEnforcerResult
    {
        public bool Success { get; set; }
        public List<IMatchingRoleEnforcerResultMember> Members { get; } = [];
    }
}
