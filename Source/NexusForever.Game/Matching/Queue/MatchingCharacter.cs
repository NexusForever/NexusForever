using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Matching.Match;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingCharacter : IMatchingCharacter
    {
        public ulong CharacterId { get; private set; }

        private readonly Dictionary<Static.Matching.MatchType, IMatchingCharacterQueue> matchingCharacterGroups = [];

        #region Dependency Injection

        private readonly ILogger<MatchingCharacter> log;
        private readonly IPlayerManager playerManager;
        private readonly IMatchManager matchManager;

        public MatchingCharacter(
            ILogger<MatchingCharacter> log,
            IPlayerManager playerManager,
            IMatchManager matchManager)
        {
            this.log           = log;
            this.playerManager = playerManager;
            this.matchManager  = matchManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchingCharacter"/> with supplied character id.
        /// </summary>
        public void Initialise(ulong characterId)
        {
            if (CharacterId != 0)
                throw new InvalidOperationException();

            CharacterId = characterId;

            log.LogTrace($"Matching information initialised for character {characterId}.");
        }

        /// <summary>
        /// Return <see cref="IMatchingCharacterQueue"/> containing the information on queue status for supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        public IMatchingCharacterQueue GetMatchingCharacterQueue(Static.Matching.MatchType matchType)
        {
            return matchingCharacterGroups.TryGetValue(matchType, out IMatchingCharacterQueue matchingCharacterQueue) ? matchingCharacterQueue : null;
        }

        public IEnumerable<IMatchingCharacterQueue> GetMatchingCharacterQueues()
        {
            return matchingCharacterGroups.Values;
        }

        /// <summary>
        /// Start tracking a <see cref="IMatchingQueueProposal"/> and <see cref="IMatchingQueueGroup"/> for character.
        /// </summary>
        public void AddMatchingQueueProposal(IMatchingQueueProposal matchingQueueProposal, IMatchingQueueGroup matchingQueueGroup)
        {
            if (matchingCharacterGroups.ContainsKey(matchingQueueProposal.MatchType))
                throw new InvalidOperationException();

            matchingCharacterGroups.Add(matchingQueueProposal.MatchType, new MatchingCharacterQueue
            {
                MatchingQueueProposal = matchingQueueProposal,
                MatchingQueueGroup    = matchingQueueGroup,
            });

            log.LogTrace($"Added matching queue proposal for {matchingQueueProposal.MatchType} to matching character {CharacterId}.");
        }

        /// <summary>
        /// Stop tracking a <see cref="IMatchingQueueProposal"/> and <see cref="IMatchingQueueGroup"/> for character of supplied <see cref="Static.Matching.MatchType"/>.
        /// </summary>
        /// <remarks>
        /// An optional <see cref="MatchingQueueResult"/> can be supplied to indicate the reason for leaving the queue.
        /// </remarks>
        public void RemoveMatchingQueueProposal(Static.Matching.MatchType matchType, MatchingQueueResult? leaveReason = MatchingQueueResult.Left)
        {
            if (!matchingCharacterGroups.ContainsKey(matchType))
                throw new InvalidOperationException();

            matchingCharacterGroups.Remove(matchType);

            if (leaveReason != null)
            {
                Send(new ServerMatchingQueueResult()
                {
                    Result = leaveReason.Value,
                });
            }

            SendMatchingStatus();

            log.LogTrace($"Removed matching queue proposal for {matchType} from matching character {CharacterId}.");
        }

        /// <summary>
        /// Send match and queue status for character.
        /// </summary>
        public void SendMatchingStatus()
        {
            Static.Matching.MatchType currentMatchType = matchManager.GetMatchCharacter(CharacterId).Match?.MatchingMap.GameTypeEntry.MatchTypeEnum
                ?? Static.Matching.MatchType.None;

            var matchingQueueLeave = new ServerMatchingStatus()
            {
                Unknown   = 0,
                MatchType = currentMatchType,
                Unknown8  = Static.Matching.MatchType.None,
            };

            // bit mask of all match types this character is queued for
            foreach (Static.Matching.MatchType existingMatchType in matchingCharacterGroups.Keys)
                matchingQueueLeave.Mask.SetBit((uint)existingMatchType, true);

            Send(matchingQueueLeave);
        }

        private void Send(IWritable message)
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);
            player?.Session.EnqueueMessageEncrypted(message);
        }
    }
}
