using NexusForever.Game.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingRoleCheck : IUpdate
    {
        Guid Guid { get; }
        MatchingRoleCheckStatus Status { get; }
        IMatchingQueueProposal MatchingQueueProposal { get; }

        /// <summary>
        /// Initialise <see cref="IMatchingRoleCheck"/> with the supplied <see cref="IMatchingQueueProposal"/> and character ids.
        /// </summary>
        void Initialise(IMatchingQueueProposal matchingQueueProposal, List<ulong> characterIds);

        IEnumerable<IMatchingRoleCheckMember> GetMembers();

        /// <summary>
        /// Set role response for supplied character id and <see cref="Role"/>.
        /// </summary>
        /// <remarks>
        /// This will also update the <see cref="MatchingRoleCheckStatus"/> to <see cref="MatchingRoleCheckStatus.Success"/> if all members have responded or <see cref="MatchingRoleCheckStatus.Declined"/> if any member has declined.
        /// </remarks>
        void Respond(ulong characterId, Role roles);
    }
}
