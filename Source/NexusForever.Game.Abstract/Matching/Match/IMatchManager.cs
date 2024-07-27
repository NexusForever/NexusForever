using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchManager : IUpdate
    {
        /// <summary>
        /// Create a new <see cref="IMatchProposal"/> for the supplied <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        void CreateMatchProposal(IMatchingQueueGroup matchingQueueGroup, IMatchingMapSelectorResult matchingMapSelectorResult);

        /// <summary>
        /// Return <see cref="IMatchProposal"/> the supplied character is currently responding to.
        /// </summary>
        IMatchProposal GetMatchProposal(ulong characterId);

        /// <summary>
        /// Return <see cref="IMatch"/> the supplied character is currently in.
        /// </summary>
        IMatch GetMatch(ulong characterId);
    }
}
