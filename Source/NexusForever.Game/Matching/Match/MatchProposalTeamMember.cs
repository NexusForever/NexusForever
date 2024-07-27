using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;

namespace NexusForever.Game.Matching.Match
{
    public class MatchProposalTeamMember : IMatchProposalTeamMember
    {
        public IMatchingQueueProposalMember MatchingQueueProposalMember { get; private set; }
        public bool? Response { get; private set; }

        #region Dependency Injection

        private readonly ILogger<MatchProposalTeamMember> log;

        public MatchProposalTeamMember(
            ILogger<MatchProposalTeamMember> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchProposalTeamMember"/> with supplied <see cref="IMatchingQueueProposalMember"/>.
        /// </summary>
        public void Initialise(IMatchingQueueProposalMember matchingQueueProposalMember)
        {
            if (MatchingQueueProposalMember != null)
                throw new InvalidOperationException();

            MatchingQueueProposalMember = matchingQueueProposalMember;
        }

        /// <summary>
        /// Set response for this <see cref="IMatchProposalTeam"/>.
        /// </summary>
        public void SetResponse(bool response)
        {
            if (Response.HasValue)
                throw new InvalidOperationException();

            Response = response;

            log.LogTrace($"Character id {MatchingQueueProposalMember.CharacterId} has responded with {Response}.");
        }
    }
}
