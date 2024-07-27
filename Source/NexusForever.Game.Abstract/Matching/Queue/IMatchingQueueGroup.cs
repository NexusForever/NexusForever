using NexusForever.Game.Static.Matching;
using NexusForever.Game.Static.Reputation;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingQueueGroup
    {
        Guid Guid { get; }
        Static.Matching.MatchType MatchType { get; }

        bool IsFinalised { get; }

        /// <summary>
        /// Determines if the <see cref="IMatchingQueueGroup"/> is paused.
        /// </summary>
        /// <remarks>
        /// When paused, the <see cref="IMatchingQueueGroup"/> will not be matched.
        /// </remarks>
        bool IsPaused { get; set; }

        /// <summary>
        /// Determines if the <see cref="IMatchingQueueGroup"/> is from a looking for replacements request.
        /// </summary>
        bool InProgress { get; }

        /// <summary>
        /// Determines if the <see cref="IMatchingQueueGroup"/> is from a solo queue join request.
        /// </summary>
        /// <remarks>
        /// This is only possible for Expeditions and is a UI option for players joining the queue.
        /// </remarks>
        bool IsSolo { get; }

        /// <summary>
        /// Initialise <see cref="IMatchingQueueGroup"/> with supplied <see cref="Static.Matching.MatchType"/> and <see cref="Faction"/>.
        /// </summary>
        void Initialise(Static.Matching.MatchType matchType, Faction faction);

        /// <summary>
        /// Return <see cref="IMatchingQueueGroupTeam"/> for the supplied <see cref="Faction"/>.
        /// </summary>
        IMatchingQueueGroupTeam GetTeam(Faction faction);

        IEnumerable<IMatchingQueueGroupTeam> GetTeams();

        /// <summary>
        /// Return common <see cref="IMatchingMap"/>'s between all <see cref="IMatchingQueueGroupTeam"/>'s.
        /// </summary>
        /// <remarks>
        /// When a new <see cref="IMatchingQueueProposal"/> is added or removed, this cache is updated.
        /// </remarks>
        List<IMatchingMap> GetMatchingMaps();

        /// <summary>
        /// Add <see cref="IMatchingQueueProposal"/> to <see cref="IMatchingQueueGroup"/>.
        /// </summary>
        void AddMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal);

        /// <summary>
        /// Remove <see cref="IMatchingQueueProposal"/> from <see cref="IMatchingQueueGroup"/>, optionally with a <see cref="MatchingQueueResult"/>.
        /// </summary>
        void RemoveMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal, MatchingQueueResult? leaveReason = MatchingQueueResult.Left);
    }
}
