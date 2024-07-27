using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Network.Message;
using NexusForever.Shared;

namespace NexusForever.Game.Matching.Match
{
    public class MatchProposalTeam : IMatchProposalTeam
    {
        public uint MemberCount => (uint)members.Count;
        public bool TeamReady { get; private set; }

        private IMatchingQueueGroupTeam team;
        private readonly Dictionary<ulong, IMatchProposalTeamMember> members = [];

        #region Dependency Injection

        private readonly IFactory<IMatchProposalTeamMember> matchProposalTeamMemberFactory;

        public MatchProposalTeam(
            IFactory<IMatchProposalTeamMember> matchProposalTeamMemberFactory)
        {
            this.matchProposalTeamMemberFactory = matchProposalTeamMemberFactory;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchProposalTeam"/> with supplied <see cref="IMatchingQueueGroupTeam"/>.
        /// </summary>
        public void Initialise(IMatchingQueueGroupTeam team)
        {
            this.team = team;

            foreach (IMatchingQueueProposalMember matchingQueueProposalMember in team.GetMembers())
            {
                IMatchProposalTeamMember matchProposalTeamMember = matchProposalTeamMemberFactory.Resolve();
                matchProposalTeamMember.Initialise(matchingQueueProposalMember);
                members.Add(matchProposalTeamMember.MatchingQueueProposalMember.CharacterId, matchProposalTeamMember);
            }
        }

        public IEnumerable<IMatchProposalTeamMember> GetMembers()
        {
            return members.Values;
        }

        /// <summary>
        /// Update response for character id.
        /// </summary>
        public void Respond(ulong characterId, bool response)
        {
            if (!members.TryGetValue(characterId, out IMatchProposalTeamMember matchProposalTeamMember))
                return;

            matchProposalTeamMember.SetResponse(response);

            TeamReady = members.Values.All(x => x.Response.HasValue && x.Response.Value == true);
        }

        /// <summary>
        /// Broadcast message to all members.
        /// </summary>
        public void Broadcast(IWritable message)
        {
            team.Broadcast(message);
        }
    }
}
