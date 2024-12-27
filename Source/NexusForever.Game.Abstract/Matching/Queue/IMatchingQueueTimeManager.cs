namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueTimeManager
    {
        /// <summary>
        /// Initialise <see cref="IMatchingQueueTimeManager"/> by creating a <see cref="IMatchingQueueTime"/> for each <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Return the average wait time for the supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        TimeSpan GetAdverageWaitTime(Static.Matching.MatchType matchType);

        /// <summary>
        /// Update average wait time with samples from <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        /// <remarks>
        /// If the <see cref="IMatchingQueueGroup"/> has multiple <see cref="IMatchingQueueProposal"/>s, the average wait time will be calculated for each <see cref="IMatchingQueueProposal"/>.
        /// </remarks>
        void Update(IMatchingQueueGroup matchingQueueGroup);
    }
}
