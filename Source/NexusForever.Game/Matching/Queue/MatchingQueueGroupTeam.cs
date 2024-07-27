using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Matching;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Game.Matching.Queue
{
    /// <summary>
    /// Represents a collection of matched <see cref="IMatchingQueueProposal"/>.
    /// </summary>
    public class MatchingQueueGroupTeam : IMatchingQueueGroupTeam
    {
        public Faction Faction { get; set; }

        private readonly HashSet<IMatchingQueueProposal> matchingQueueProposals = [];

        #region Dependency Injection

        private readonly ILogger<MatchingQueueGroupTeam> log;

        public MatchingQueueGroupTeam(
            ILogger<MatchingQueueGroupTeam> log)
        {
            this.log = log;
        }

        #endregion

        /// <summary>
        /// Add <see cref="IMatchingQueueProposal"/> to the team.
        /// </summary>
        public void AddMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal)
        {
            matchingQueueProposals.Add(matchingQueueProposal);
        }

        /// <summary>
        /// Remove <see cref="IMatchingQueueProposal"/> from the team.
        /// </summary>
        public void RemoveMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal)
        {
            matchingQueueProposals.Remove(matchingQueueProposal);
        }

        /// <summary>
        /// Return a distinct common collection of <see cref="IMatchingMap"/>s from the underlying <see cref="IMatchingQueueProposal"/>s.
        /// </summary>
        public IEnumerable<IMatchingMap> GetMatchingMaps()
        {
            return matchingQueueProposals
                .SelectMany(m => m.GetMatchingMaps())
                .Distinct();
        }

        /// <summary>
        /// Return a collection of <see cref="IMatchingQueueProposalMember"/>s from the underlying <see cref="IMatchingQueueProposal"/>s.
        /// </summary>
        public IEnumerable<IMatchingQueueProposalMember> GetMembers()
        {
            return matchingQueueProposals.SelectMany(m => m.GetMembers());
        }

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all <see cref="IMatchingQueueProposal"/>s.
        /// </summary>
        public void Broadcast(IWritable message)
        {
            foreach (IMatchingQueueProposal party in matchingQueueProposals)
                party.Broadcast(message);
        }
    }
}
