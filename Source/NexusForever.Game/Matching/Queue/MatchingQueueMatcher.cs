using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching.Queue;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingQueueMatcher : IMatchingQueueMatcher
    {
        #region Dependency Injection

        private readonly ILogger<MatchingQueueMatcher> log;
        private readonly IMatchingQueueGroupMatcher matchingQueueGroupMatcher;

        public MatchingQueueMatcher(
            ILogger<MatchingQueueMatcher> log,
            IMatchingQueueGroupMatcher matchingQueueGroupMatcher)
        {
            this.log                       = log;
            this.matchingQueueGroupMatcher = matchingQueueGroupMatcher;
        }

        #endregion

        /// <summary>
        /// Attempt to match a <see cref="IMatchingQueueProposal"/> against a list of <see cref="IMatchingQueueGroup"/>s.
        /// </summary>
        /// <remarks>
        /// A matched <see cref="IMatchingQueueGroup"/> can be an incomplete group.
        /// </remarks>
        public IMatchingQueueGroup Match(List<IMatchingQueueGroup> matchingQueueGroups, IMatchingQueueProposal matchingQueueProposal)
        {
            log.LogTrace($"Matching queue proposal {matchingQueueProposal.Guid} against {matchingQueueGroups.Count} matching queue groups...");

            // TODO: how smart should this be?
            // currently we just match with the first matching queue group with valid criteria
            // might be able to take into queue times, priorty, etc...
            foreach (IMatchingQueueGroup matchingQueueGroup in matchingQueueGroups)
            {
                if (matchingQueueGroup.IsPaused)
                    continue;

                if (matchingQueueGroupMatcher.Match(matchingQueueGroup, matchingQueueProposal))
                {
                    log.LogTrace($"Successfully matched matching queue proposal {matchingQueueProposal.Guid} to matching queue group {matchingQueueGroup.Guid}.");
                    return matchingQueueGroup;
                }
            }

            return null;
        }
    }
}
