using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingQueueGroupMatcher : IMatchingQueueGroupMatcher
    {
        #region Dependency Injection

        private readonly ILogger<MatchingQueueGroupMatcher> log;

        private readonly IMatchingDataManager matchingDataManager; 
        private readonly IMatchingRoleEnforcer matchingRoleEnforcer;

        public MatchingQueueGroupMatcher(
            ILogger<MatchingQueueGroupMatcher> log,
            IMatchingDataManager matchingDataManager,
            IMatchingRoleEnforcer matchingRoleEnforcer)
        {
            this.log                  = log;

            this.matchingDataManager  = matchingDataManager;
            this.matchingRoleEnforcer = matchingRoleEnforcer;
        }

        #endregion

        /// <summary>
        /// Attempt to match <see cref="IMatchingQueueProposal"/> against <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        public bool Match(IMatchingQueueGroup matchingQueueGroup, IMatchingQueueProposal matchingQueueProposal)
        {
            log.LogTrace($"Attempting to match matching queue proposal {matchingQueueProposal.Guid} against matching queue group {matchingQueueGroup.Guid}.");

            IEnumerable<IMatchingMap> commonMatchingMaps = matchingQueueGroup.GetMatchingMaps()
                .Intersect(matchingQueueProposal.GetMatchingMaps());

            if (!commonMatchingMaps.Any())
                return false;

            IMatchingQueueGroupTeam matchingQueueGroupTeam = matchingQueueGroup.GetTeam(matchingQueueProposal.Faction);
            return Match(commonMatchingMaps, matchingQueueGroupTeam, matchingQueueProposal);
        }

        private bool Match(IEnumerable<IMatchingMap> commonMatchingMaps, IMatchingQueueGroupTeam matchingQueueGroupTeam, IMatchingQueueProposal matchingQueueProposal)
        {
            List<IMatchingQueueProposalMember> matchingQueueProposalMembers = matchingQueueGroupTeam
                .GetMembers()
                .Concat(matchingQueueProposal.GetMembers())
                .ToList();

            foreach (IMatchingMap matchingMap in commonMatchingMaps)
            {
                if (Match(matchingMap, matchingQueueProposalMembers))
                    return true;
            }

            return false;
        }

        private bool Match(IMatchingMap matchingMap, List<IMatchingQueueProposalMember> matchingQueueProposalMembers)
        {
            if (matchingQueueProposalMembers.Count > matchingMap.GameTypeEntry.TeamSize)
                return false;

            if (matchingDataManager.IsCompositionEnforced(matchingMap.GameTypeEntry.MatchTypeEnum))
            {
                IMatchingRoleEnforcerResult result = matchingRoleEnforcer.Check(matchingQueueProposalMembers);
                if (!result.Success)
                    return false;
            }

            // additional checks needed for match?
            // TODO: MMR?
            // TODO: Ignore list?

            return true;
        }
    }
}
