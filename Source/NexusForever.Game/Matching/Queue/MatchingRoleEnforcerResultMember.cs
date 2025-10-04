using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingRoleEnforcerResultMember : IMatchingRoleEnforcerResultMember
    {
        public required Identity Identity { get; init; }
        public required Role Role { get; set; }
    }
}
