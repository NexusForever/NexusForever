namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueMatcher
    {
        /// <summary>
        /// Attempt to match a <see cref="IMatchingQueueProposal"/> against a list of <see cref="IMatchingQueueGroup"/>s.
        /// </summary>
        /// <remarks>
        /// A matched <see cref="IMatchingQueueGroup"/> can be an incomplete group.
        /// </remarks>
        (IMatchingQueueGroup, IMatchingQueueGroupTeam)? Match(List<IMatchingQueueGroup> matchingQueueGroups, IMatchingQueueProposal matchingQueueProposal);
    }
}
