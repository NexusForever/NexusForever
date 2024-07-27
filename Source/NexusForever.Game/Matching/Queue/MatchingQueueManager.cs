using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingQueueManager : IMatchingQueueManager
    {
        private Dictionary<Static.Matching.MatchType, IMatchingQueue> queues = [];

        #region Dependency Injection

        private readonly IFactory<IMatchingQueue> queueFactory;
        private readonly IFactory<IMatchingQueueValidator> matchingQueueValidatorFactory;

        public MatchingQueueManager(
            IFactory<IMatchingQueue> queueFactory,
            IFactory<IMatchingQueueValidator> matchingQueueValidatorFactory)
        {
            this.queueFactory                  = queueFactory;
            this.matchingQueueValidatorFactory = matchingQueueValidatorFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchingQueueManager"/> with <see cref="IMatchingQueue"/>s for each <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        public void Initialise()
        {
            foreach (var type in Enum.GetValues<Static.Matching.MatchType>())
            {
                IMatchingQueue matchingQueue = queueFactory.Resolve();
                matchingQueue.Initialise(type);
                queues.Add(type, matchingQueue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update(double lastTick)
        {
            foreach (IMatchingQueue matchingQueue in queues.Values)
                matchingQueue.Update(lastTick);
        }

        /// <summary>
        /// Determines if <see cref="IMatchingQueueProposal"/> is valid to be queued.
        /// </summary>
        public MatchingQueueResult? CanQueue(IMatchingQueueProposal matchingQueueProposal)
        {
            IMatchingQueueValidator matchingQueueValidator = matchingQueueValidatorFactory.Resolve();
            return matchingQueueValidator.CanQueue(matchingQueueProposal);
        }

        /// <summary>
        /// Attempt to add <see cref="IMatchingQueueProposal"/> to the queue.
        /// </summary>
        public void JoinQueue(IMatchingQueueProposal matchingParty)
        {
            if (!queues.TryGetValue(matchingParty.MatchType, out IMatchingQueue queue))
                throw new InvalidOperationException();

            queue.JoinQueue(matchingParty);
        }
    }
}
