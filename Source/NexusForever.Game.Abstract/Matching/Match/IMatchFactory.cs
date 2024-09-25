namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchFactory
    {
        /// <summary>
        /// Create <see cref="IMatch"/> for supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        IMatch CreateMatch(Static.Matching.MatchType matchType);
    }
}
