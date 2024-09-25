using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Match
{
    public class MatchFactory : IMatchFactory
    {
        #region Dependency Injection

        private readonly IMatchingDataManager matchingDataManager;
        private readonly IFactoryInterface<IMatch> matchFactory;

        public MatchFactory(
            IMatchingDataManager matchingDataManager,
            IFactoryInterface<IMatch> matchFactory)
        {
            this.matchingDataManager = matchingDataManager;
            this.matchFactory = matchFactory;
        }

        #endregion

        /// <summary>
        /// Create <see cref="IMatch"/> for supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        public IMatch CreateMatch(Static.Matching.MatchType matchType)
        {
            if (matchingDataManager.IsPvPMatchType(matchType))
                return matchFactory.Resolve<IPvpMatch>();

            return matchFactory.Resolve<IMatch>();
        }
    }
}
