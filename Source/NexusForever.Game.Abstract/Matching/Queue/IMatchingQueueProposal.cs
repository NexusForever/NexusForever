using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueProposal
    {
        Guid Guid { get; }
        Faction Faction { get; }
        Static.Matching.MatchType MatchType { get; }
        MatchingQueueFlags MatchingQueueFlags { get; }
        DateTime QueueTime { get; }

        /// <summary>
        /// Determines if the <see cref="IMatchingQueueProposal"/> was formed from a party.
        /// </summary>
        bool IsParty { get; }

        /// <summary>
        /// Initialise a new <see cref="IMatchingQueueProposal"/>.
        /// </summary>
        void Initialise(Faction faction, Static.Matching.MatchType matchType, IEnumerable<IMatchingMap> matchingMaps, MatchingQueueFlags matchingQueueFlags);

        /// <summary>
        /// Add a new member to the <see cref="IMatchingQueueProposal"/> with supplied character id and <see cref="Role"/>.
        /// </summary>
        void AddMember(ulong characterId, Role roles);

        /// <summary>
        /// Return <see cref="IMatchingQueueProposalMember"/> for the supplied character id.
        /// </summary>
        IMatchingQueueProposalMember GetMember(ulong characterId);

        IEnumerable<IMatchingQueueProposalMember> GetMembers();
        IEnumerable<IMatchingMap> GetMatchingMaps();

        /// <summary>
        /// Set <see cref="IMatchingQueueGroup"/> for the <see cref="IMatchingQueueProposal"/>.
        /// </summary>
        void SetMatchingQueueGroup(IMatchingQueueGroup matchingQueueGroup);

        /// <summary>
        /// Remove <see cref="IMatchingQueueGroup"/> from <see cref="IMatchingQueueProposal"/>, optionally with a <see cref="MatchingQueueResult"/>.
        /// </summary>
        void RemoveMatchingQueueGroup(MatchingQueueResult? leaveReason = MatchingQueueResult.Left);

        /// <summary>
        /// Broadcast <see cref="IWritable"/> to all members.
        /// </summary>
        void Broadcast(IWritable message);
    }
}
