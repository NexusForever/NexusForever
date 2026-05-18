using NexusForever.Game.Abstract.Matching.Queue;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingCharacterQueue : IMatchingCharacterQueue
    {
        public IMatchingQueueProposal MatchingQueueProposal { get; init; }
        public IMatchingQueueGroup MatchingQueueGroup { get; init; }
    }
}
