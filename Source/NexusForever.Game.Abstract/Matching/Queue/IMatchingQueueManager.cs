using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueManager : IUpdate
    {
        /// <summary>
        /// Initialise <see cref="IMatchingQueueManager"/> with <see cref="IMatchingQueue"/>s for each <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Determines if <see cref="IMatchingQueueProposal"/> is valid to be queued.
        /// </summary>
        MatchingQueueResult? CanQueue(IMatchingQueueProposal matchingGroup);

        /// <summary>
        /// Attempt to add <see cref="IMatchingQueueProposal"/> to the queue.
        /// </summary>
        void JoinQueue(IMatchingQueueProposal matchingGroup);
    }
}
