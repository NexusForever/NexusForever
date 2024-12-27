using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingMapSelectorResult : IMatchingMapSelectorResult
    {
        public IMatchingMap MatchingMap { get; init; }
        public Dictionary<Guid, IMatchingRoleEnforcerResult> MatchingRoleEnforcerResults { get; } = [];
    }
}
