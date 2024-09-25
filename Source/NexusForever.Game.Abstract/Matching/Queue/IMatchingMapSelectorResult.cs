namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingMapSelectorResult
    {
        IMatchingMap MatchingMap { get; init; }
        Dictionary<Guid, IMatchingRoleEnforcerResult> MatchingRoleEnforcerResults { get; }
    }
}
