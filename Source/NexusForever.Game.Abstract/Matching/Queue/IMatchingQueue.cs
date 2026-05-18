using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueue : IUpdate
    {
        /// <summary>
        /// Initialise <see cref="IMatchingQueue"/> with supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        void Initialise(Static.Matching.MatchType matchType);

        /// <summary>
        /// Attempt to add <see cref="IMatchingQueueProposal"/> to the queue.
        /// </summary>
        /// <remarks>
        /// An attempt to match the <see cref="IMatchingQueueProposal"/> with any <see cref="IMatchingQueueGroup"/>s already in queue, otherwise add to queue.
        /// </remarks>
        void JoinQueue(IMatchingQueueProposal matchingGroup);
    }
}
