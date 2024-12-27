using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueGroupTeam
    {
        Guid Guid { get; }

        /// <summary>
        /// <see cref="Static.Reputation.Faction"/> of the <see cref="IMatchingQueueGroupTeam"/>.
        /// </summary>
        /// <remarks>
        /// Faction is optional and will only be set when <see cref="IMatchingDataManager.IsSingleFactionEnforced"/> is true for <see cref="MatchType"/>, otherwise team will be made up of members from both factions.
        /// </remarks>
        Faction? Faction { get; }

        /// <summary>
        /// Initialise <see cref="IMatchingQueueGroupTeam"/> with optional <see cref="Static.Reputation.Faction"/>.
        /// </summary>
        void Initialise(Faction? faction);

        /// <summary>
        /// Add <see cref="IMatchingQueueProposal"/> to the team.
        /// </summary>
        void AddMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal);

        /// <summary>
        /// Remove <see cref="IMatchingQueueProposal"/> from the team.
        /// </summary>
        void RemoveMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal);

        /// <summary>
        /// Return a distinct common collection of <see cref="IMatchingMap"/>s from the underlying <see cref="IMatchingQueueProposal"/>s.
        /// </summary>
        IEnumerable<IMatchingMap> GetMatchingMaps();

        /// <summary>
        /// Return a collection of <see cref="IMatchingQueueProposalMember"/>s from the underlying <see cref="IMatchingQueueProposal"/>s.
        /// </summary>
        IEnumerable<IMatchingQueueProposalMember> GetMembers();

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all <see cref="IMatchingQueueProposal"/>s.
        /// </summary>
        void Broadcast(IWritable message);
    }
}
