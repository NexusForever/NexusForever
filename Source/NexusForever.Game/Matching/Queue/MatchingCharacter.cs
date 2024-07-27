using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
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

        public MatchingCharacter(
            ILogger<MatchingCharacter> log,
            IPlayerManager playerManager)
        {
            this.log           = log;
            this.playerManager = playerManager;
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

            var matchingQueueLeave = new ServerMatchingQueueLeave()
            {
                Unknown  = 0,
                Unknown4 = Static.Matching.MatchType.None,
                Unknown8 = Static.Matching.MatchType.None,
            };

            // bit mask of all match types this character is queued for
            foreach (Static.Matching.MatchType existingMatchType in matchingCharacterGroups.Keys)
                matchingQueueLeave.Mask.SetBit((uint)existingMatchType, true);

            Send(matchingQueueLeave);

            log.LogTrace($"Removed matching queue proposal for {matchType} from matching character {CharacterId}.");
        }

        private void Send(IWritable message)
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);
            player?.Session.EnqueueMessageEncrypted(message);
        }
    }
}
