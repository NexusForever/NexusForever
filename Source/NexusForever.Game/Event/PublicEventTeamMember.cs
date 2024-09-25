using Microsoft.Extensions.Logging;
using NexusForever.Game.Abstract;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Event;
using NexusForever.Game.Static.Event;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Event
{
    public class PublicEventTeamMember : IPublicEventTeamMember
    {
        public ulong CharacterId { get; private set; }

        private Static.Entity.Class @class;
        private Static.Entity.Path path;

        private readonly IPublicEventStats stats = new PublicEventStats();

        #region Dependency Injection

        private readonly ILogger<PublicEventTeamMember> log;

        private readonly IPlayerManager playerManager;
        private readonly IRealmContext realmContext;

        public PublicEventTeamMember(
            ILogger<PublicEventTeamMember> log,
            IPlayerManager playerManager,
            IRealmContext realmContext)
        {
            this.log           = log;
            this.playerManager = playerManager;
            this.realmContext  = realmContext;
        }

        #endregion

        /// <summary>
        /// Initialise <see cref="PublicEventTeamMember"/> with <see cref="IPlayer"/>.
        /// </summary>
        public void Initialise(IPlayer player)
        {
            if (CharacterId != 0)
                throw new InvalidOperationException();

            CharacterId = player.CharacterId;
            @class      = player.Class;
            path        = player.Path;

            log.LogTrace($"Initialised public event team member {CharacterId} with class {@class} and path {path}.");
        }

        /// <summary>
        /// Update <see cref="PublicEventStat"/> with supplied value.
        /// </summary>
        public void UpdateStat(PublicEventStat stat, uint value)
        {
            stats.UpdateStat(stat, value);
            log.LogTrace($"Updated public event team member {CharacterId} stat {stat} to {value}.");
        }

        /// <summary>
        /// Update custom stat with supplied value.
        /// </summary>
        public void UpdateCustomStat(uint index, uint value)
        {
            stats.UpdateCustomStat(index, value);
            log.LogTrace($"Updated public event team member {CharacterId} custom stat {index} to {value}.");
        }

        /// <summary>
        /// Send <see cref="IWritable"/> to <see cref="IPublicEventTeamMember"/>.
        /// </summary>
        public void Send(IWritable message)
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);
            player?.Session.EnqueueMessageEncrypted(message);
        }

        /// <summary>
        /// Build <see cref="Network.World.Message.Model.Shared.PublicEventStats"/> for <see cref="IPublicEventTeamMember"/>.
        /// </summary>
        public Network.World.Message.Model.Shared.PublicEventStats BuildStats()
        {
            return stats.Build();
        }

        /// <summary>
        /// Build <see cref="PublicEventParticipantStats"/> for <see cref="IPublicEventTeamMember"/>.
        /// </summary>
        public PublicEventParticipantStats BuildParticipantStats()
        {
            IPlayer player = playerManager.GetPlayer(CharacterId);

            return new PublicEventParticipantStats
            {
                UnitId = player.Guid,
                Player = new TargetPlayerIdentity
                {
                    CharacterId = player.CharacterId,
                    RealmId     = realmContext.RealmId
                },
                Class = @class,
                Path  = path,
                Stats = BuildStats()
            };
        }
    }
}
