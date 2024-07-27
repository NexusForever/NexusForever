using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Match
{
    public interface IMatchProposalTeam
    {
        uint MemberCount { get; }
        bool TeamReady { get; }

        /// <summary>
        /// Initialise <see cref="IMatchProposalTeam"/> with supplied <see cref="IMatchingQueueGroupTeam"/>.
        /// </summary>
        void Initialise(IMatchingQueueGroupTeam team);

        IEnumerable<IMatchProposalTeamMember> GetMembers();

        /// <summary>
        /// Update response for character id.
        /// </summary>
        void Respond(ulong characterId, bool response);

        /// <summary>
        /// Broadcast message to all members.
        /// </summary>
        void Broadcast(IWritable message);
    }
}
