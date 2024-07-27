using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingMapSelectorResult
    {
        IMatchingMap MatchingMap { get; init; }
        Dictionary<Faction, IMatchingRoleEnforcerResult> MatchingRoleEnforcerResults { get; }
    }
}