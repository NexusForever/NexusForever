using NexusForever.Game.Abstract.Matching.Queue;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchProposalTeamMember
    {
        IMatchingQueueProposalMember MatchingQueueProposalMember { get; }
        bool? Response { get; }

        /// <summary>
        /// Initialise <see cref="IMatchProposalTeamMember"/> with supplied <see cref="IMatchingQueueProposalMember"/>.
        /// </summary>
        void Initialise(IMatchingQueueProposalMember matchingQueueProposalMember);

        /// <summary>
        /// Set response for this <see cref="IMatchProposalTeam"/>.
        /// </summary>
        void SetResponse(bool response);
    }
}
