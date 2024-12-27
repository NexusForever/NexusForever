using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueProposalMember
    {
        IMatchingQueueProposal MatchingQueueProposal { get; }

        ulong CharacterId { get; }
        Role Roles { get; }

        /// <summary>
        /// Initialise <see cref="IMatchingQueueProposalMember"/>.
        /// </summary>
        void Initialise(IMatchingQueueProposal matchingQueueProposal, ulong characterId, Role roles);

        /// <summary>
        /// Send <see cref="IWritable"/> to member.
        /// </summary>
        void Send(IWritable message);
    }
}
