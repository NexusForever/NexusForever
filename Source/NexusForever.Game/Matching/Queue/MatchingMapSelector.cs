using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingMapSelector : IMatchingMapSelector
    {
        #region Dependency Injection

        private readonly IMatchingDataManager matchingDataManager;
        private readonly IMatchingRoleEnforcer matchingRoleEnforcer;

        public MatchingMapSelector(
            IMatchingDataManager matchingDataManager,
            IMatchingRoleEnforcer matchingRoleEnforcer)
        {
            this.matchingDataManager = matchingDataManager;
            this.matchingRoleEnforcer = matchingRoleEnforcer;
        }

        #endregion

        /// <summary>
        /// Determines the best <see cref="IMatchingMap"/> and role composition for the given <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        public IMatchingMapSelectorResult Select(IMatchingQueueGroup matchingQueueGroup)
        {
            var validMaps = new List<IMatchingMap>();
            foreach (IMatchingMap matchingMap in matchingQueueGroup.GetMatchingMaps())
            {
                if (CanSelect(matchingQueueGroup, matchingMap))
                    validMaps.Add(matchingMap);
            }

            if (validMaps.Count == 0)
                return null;

            return Select(matchingQueueGroup, validMaps);
        }

        private bool CanSelect(IMatchingQueueGroup matchingQueueGroup, IMatchingMap matchingMap)
        {
            foreach (IMatchingQueueGroupTeam item in matchingQueueGroup.GetTeams())
            {
                if (item.GetMembers().Count() != matchingMap.GameTypeEntry.TeamSize)
                    return false;
            }

            return true;
        }

        private IMatchingMapSelectorResult Select(IMatchingQueueGroup matchingQueueGroup, List<IMatchingMap> matchingMaps)
        {
            var matchingMapSelectorResult = new MatchingMapSelectorResult
            {
                MatchingMap = matchingMaps[Random.Shared.Next(matchingMaps.Count)]
            };

            if (matchingDataManager.IsCompositionEnforced(matchingMapSelectorResult.MatchingMap.GameTypeEntry.MatchTypeEnum))
            {
                foreach (IMatchingQueueGroupTeam matchingQueueGroupTeam in matchingQueueGroup.GetTeams())
                {
                    IMatchingRoleEnforcerResult matchingRoleEnforcerResult = matchingRoleEnforcer.Check(matchingQueueGroupTeam.GetMembers());
                    matchingMapSelectorResult.MatchingRoleEnforcerResults.Add(matchingQueueGroupTeam.Faction, matchingRoleEnforcerResult);
                }
            }

            return matchingMapSelectorResult;
        }
    }
}
