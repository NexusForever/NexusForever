using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchProposal : IUpdate
    {
        Guid Guid { get; }
        MatchProposalStatus Status { get; }
        IMatchingQueueGroup MatchingQueueGroup { get; }
        IMatchingMapSelectorResult MatchingMapSelectorResult { get; }

        /// <summary>
        /// Initialise new match proposal with supplied <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        void Initialise(IMatchingQueueGroup matchingQueueGroup, IMatchingMapSelectorResult matchingMapSelectorResult);
        
        IEnumerable<IMatchProposalTeam> GetTeams();

        /// <summary>
        /// Update match proposal response for <see cref="IPlayer"/>.
        /// </summary>
        void Respond(IPlayer player, bool response);
    }
}
