using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueGroupTeam
    {
        Faction Faction { get; set; }

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
