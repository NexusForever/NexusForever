using NexusForever.Game.Static.Matching;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingCharacter
    {
        ulong CharacterId { get; }

        /// <summary>
        /// Initialise <see cref="IMatchingCharacter"/> with supplied character id.
        /// </summary>
        void Initialise(ulong characterId);

        /// <summary>
        /// Return <see cref="IMatchingCharacterQueue"/> containing the information on queue status for supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        IMatchingCharacterQueue GetMatchingCharacterQueue(Static.Matching.MatchType matchType);

        IEnumerable<IMatchingCharacterQueue> GetMatchingCharacterQueues();

        /// <summary>
        /// Start tracking a <see cref="IMatchingQueueProposal"/> and <see cref="IMatchingQueueGroup"/> for character.
        /// </summary>
        void AddMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal, IMatchingQueueGroup matchingQueueGroup);

        /// <summary>
        /// Stop tracking a <see cref="IMatchingQueueProposal"/> and <see cref="IMatchingQueueGroup"/> for character of supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        /// <remarks>
        /// An optional <see cref="MatchingQueueResult"/> can be supplied to indicate the reason for leaving the queue.
        /// </remarks>
        void RemoveMatchingQueueProposal(Static.Matching.MatchType matchType, MatchingQueueResult? leaveReason = MatchingQueueResult.Left);
    }
}
