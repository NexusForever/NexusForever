using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Matching.Queue;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingQueueProposalMember : IMatchingQueueProposalMember
    {
        public IMatchingQueueProposal MatchingQueueProposal { get; private set; }

        public ulong CharacterId { get; private set; }
        public Role Roles { get; private set; }

        #region Dependency Injection

        private readonly IPlayerManager playerManager;

        public MatchingQueueProposalMember(
            IPlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchingQueueProposalMember"/>.
        /// </summary>
        public void Initialise(IMatchingQueueProposal matchingQueueProposal, ulong characterId, Role roles)
        {
            if (MatchingQueueProposal != null)
                throw new InvalidOperationException();

            MatchingQueueProposal = matchingQueueProposal;
            CharacterId           = characterId;
            Roles                 = roles;
        }

        /// <summary>
        /// Send <see cref="IWritable"/> to member.
        /// </summary>
        public void Send(IWritable message)
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);
            player?.Session.EnqueueMessageEncrypted(message);
        }
    }
}
