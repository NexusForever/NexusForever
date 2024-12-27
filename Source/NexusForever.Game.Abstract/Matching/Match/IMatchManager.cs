using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchManager : IUpdate
    {
        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs in.
        /// </summary>
        void OnLogin(IPlayer player);

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs out.
        /// </summary>
        void OnLogout(IPlayer player);

        /// <summary>
        /// Create a new <see cref="IMatchProposal"/> for the supplied <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        void CreateMatchProposal(IMatchingQueueGroup matchingQueueGroup, IMatchingMapSelectorResult matchingMapSelectorResult);

        /// <summary>
        /// Return <see cref="IMatchCharacter"/> for supplied character id.
        /// </summary>
        /// <remarks>
        /// Will return a new <see cref="IMatchCharacter"/> if one does not exist.
        /// </remarks>
        public IMatchCharacter GetMatchCharacter(ulong characterId);
    }
}
