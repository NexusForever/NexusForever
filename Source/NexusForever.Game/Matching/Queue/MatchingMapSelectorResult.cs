using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingMapSelectorResult : IMatchingMapSelectorResult
    {
        public IMatchingMap MatchingMap { get; init; }
        public Dictionary<Faction, IMatchingRoleEnforcerResult> MatchingRoleEnforcerResults { get; } = [];
    }
}
