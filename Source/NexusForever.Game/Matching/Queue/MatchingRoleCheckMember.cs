using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Game.Matching.Queue
{
    public class MatchingRoleCheckMember : IMatchingRoleCheckMember
    {
        public ulong CharacterId { get; private set; }
        public Role? Roles { get; private set; }

        #region Dependency Injection

        private readonly ILogger<MatchingRoleCheckMember> log;
        private readonly IPlayerManager playerManager;

        public MatchingRoleCheckMember(
            IPlayerManager playerManager,
            ILogger<MatchingRoleCheckMember> log)
        {
            this.playerManager = playerManager;
            this.log           = log;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="IMatchingRoleCheckMember"/> with supplied character id.
        /// </summary>
        public void Initialise(ulong characterId)
        {
            if (CharacterId != 0)
                throw new InvalidOperationException();

            CharacterId = characterId;
        }

        /// <summary>
        /// Set <see cref="Role"/> for member.
        /// </summary>
        public void SetRoles(Role roles)
        {
            if (Roles.HasValue)
                throw new InvalidOperationException();

            Roles = roles;

            log.LogTrace($"MatchingRoleCheckMember: {CharacterId} has responded with {Roles}.");
        }

        /// <summary>
        /// Send <see cref="IWritable"/> message to member.
        /// </summary>
        public void Send(IWritable message)
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);
            player?.Session.EnqueueMessageEncrypted(message);
        }
    }
}
