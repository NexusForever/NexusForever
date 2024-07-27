using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingQueueTimeManager : IMatchingQueueTimeManager
    {
        private readonly Dictionary<Static.Matching.MatchType, IMatchingQueueTime> matchingQueueTimes = [];

        #region Dependency Injection

        private readonly ILogger<IMatchingQueueTimeManager> log;
        private readonly IFactory<IMatchingQueueTime> matchingQueueTimeFactory;

        public MatchingQueueTimeManager(
            ILogger<IMatchingQueueTimeManager> log,
            IFactory<IMatchingQueueTime> matchingQueueTimeFactory)
        {
            this.log                      = log;
            this.matchingQueueTimeFactory = matchingQueueTimeFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchingQueueTimeManager"/> by creating a <see cref="IMatchingQueueTime"/> for each <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        public void Initialise()
        {
            foreach (Static.Matching.MatchType matchType in Enum.GetValues<Static.Matching.MatchType>())
            {
                IMatchingQueueTime matchingQueueTime = matchingQueueTimeFactory.Resolve();
                matchingQueueTime.Initialise(matchType);
                matchingQueueTimes.Add(matchType, matchingQueueTime);
            }

            log.LogTrace($"Initialised queue times for {matchingQueueTimes.Count} matach types.");
        }

        /// <summary>
        /// Return the average wait time for the supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        public TimeSpan GetAdverageWaitTime(Static.Matching.MatchType matchType)
        {
            return matchingQueueTimes[matchType].Average;
        }

        /// <summary>
        /// Update average wait time with samples from <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        /// <remarks>
        /// If the <see cref="IMatchingQueueGroup"/> has multiple <see cref="IMatchingQueueProposal"/>s, the average wait time will be calculated for each <see cref="IMatchingQueueProposal"/>.
        /// </remarks>
        public void Update(IMatchingQueueGroup matchingQueueGroup)
        {
            HashSet<IMatchingQueueProposal> matchingQueueProposals = [];
            foreach (IMatchingQueueGroupTeam matchingQueueGroupTeam in matchingQueueGroup.GetTeams())
                foreach (IMatchingQueueProposalMember matchingQueueProposalMember in matchingQueueGroupTeam.GetMembers())
                    matchingQueueProposals.Add(matchingQueueProposalMember.MatchingQueueProposal);

            if (!matchingQueueTimes.TryGetValue(matchingQueueGroup.MatchType, out IMatchingQueueTime matchingQueueTime))
                throw new ArgumentOutOfRangeException();

            foreach (IMatchingQueueProposal item in matchingQueueProposals)
            {
                TimeSpan timeInQueue = DateTime.UtcNow - item.QueueTime;
                matchingQueueTime.Update(timeInQueue);
            }
        }
    }
}
