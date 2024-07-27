namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingCharacterQueue
    {
        IMatchingQueueProposal MatchingQueueProposal { get; init; }
        IMatchingQueueGroup MatchingQueueGroup { get; init; }
    }
}
