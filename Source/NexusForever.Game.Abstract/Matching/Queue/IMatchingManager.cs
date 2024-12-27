using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Matching;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Matching.Queue
{
    public interface IMatchingManager : IUpdate
    {
        /// <summary>
        /// Initialise data and queue managers for <see cref="IMatchingManager"/>.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Return <see cref="IMatchingCharacter"/> for supplied character id.
        /// </summary>
        /// <remarks>
        /// Will return a new <see cref="IMatchingCharacter"/> if one does not exist.
        /// </remarks>
        IMatchingCharacter GetMatchingCharacter(ulong characterId);

        /// <summary>
        /// Return <see cref="IMatchingRoleCheck"/> for supplied character id.
        /// </summary>
        IMatchingRoleCheck GetMatchingRoleCheck(ulong characterId);

        /// <summary>
        /// Attempt to join a matching queue.
        /// </summary>
        void JoinQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType, List<uint> maps, uint matchingGameTypeId, MatchingQueueFlags matchingQueueFlags);

        /// <summary>
        /// Attempt to join a matching queue with a party.
        /// </summary>
        void JoinPartyQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType, List<uint> maps, uint matchingGameTypeId, MatchingQueueFlags matchingQueueFlags);

        /// <summary>
        /// Attempt to join a random queue.
        /// </summary>
        void JoinRandomQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType);

        /// <summary>
        /// Attempt to join a random party queue.
        /// </summary>
        void JoinRandomPartyQueue(IPlayer player, Role roles, Static.Matching.MatchType matchType);

        /// <summary>
        /// Remove <see cref="IPlayer"/> from specified <see cref="Static.Matching.MatchType"/> queue.
        /// </summary>
        void LeaveQueue(IPlayer player, Static.Matching.MatchType matchType);

        /// <summary>
        /// Remove <see cref="IPlayer"/> from all queues.
        /// </summary>
        void LeaveQueue(IPlayer player);

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> logs in.
        /// </summary>
        void OnLogin(IPlayer player);

        /// <summary>
        /// Invoked when <see cref="IPlayer"/> goes offline.
        /// </summary>
        void OnLogout(IPlayer player);
    }
}
