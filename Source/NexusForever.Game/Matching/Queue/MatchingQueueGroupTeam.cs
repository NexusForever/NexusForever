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
        public Guid Guid { get; private set; }

        /// <summary>
        /// <see cref="Static.Reputation.Faction"/> of the <see cref="IMatchingQueueGroupTeam"/>.
        /// </summary>
        /// <remarks>
        /// Faction is optional and will only be set when <see cref="IMatchingDataManager.IsSingleFactionEnforced"/> is true for <see cref="MatchType"/>, otherwise team will be made up of members from both factions.
        /// </remarks>
        public Faction? Faction { get; private set; }

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
        /// Initialise <see cref="IMatchingQueueGroupTeam"/> with optional <see cref="Static.Reputation.Faction"/>.
        /// </summary>
        public void Initialise(Faction? faction)
        {
            if (Guid != Guid.Empty)
                throw new InvalidOperationException();

            Guid = Guid.NewGuid();
            if (faction != null)
                Faction = faction;

            log.LogTrace($"Initialising matching queue group team {Guid}.");
        }

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
