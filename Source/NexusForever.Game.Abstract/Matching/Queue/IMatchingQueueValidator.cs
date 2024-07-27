using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueValidator
    {
        /// <summary>
        /// Determines if <see cref="IMatchingQueueProposal"/> is valid to be queued.
        /// </summary>
        MatchingQueueResult? CanQueue(IMatchingQueueProposal matchingParty);
    }
}
