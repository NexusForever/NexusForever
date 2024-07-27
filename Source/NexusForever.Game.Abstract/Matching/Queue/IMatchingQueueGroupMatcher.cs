namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueGroupMatcher
    {
        /// <summary>
        /// Attempt to match <see cref="IMatchingQueueProposal"/> against <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        bool Match(IMatchingQueueGroup matchingQueueGroup, IMatchingQueueProposal matchingParty);
    }
}
