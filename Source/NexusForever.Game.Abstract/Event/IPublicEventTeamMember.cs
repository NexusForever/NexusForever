using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Event;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventTeamMember
    {
        ulong CharacterId { get; }

        /// <summary>
        /// Initialise <see cref="IPublicEventTeamMember"/> with <see cref="IPlayer"/>.
        /// </summary>
        void Initialise(IPlayer player);

        /// <summary>
        /// Update <see cref="PublicEventStat"/> with supplied value.
        /// </summary>
        void UpdateStat(PublicEventStat stat, uint value);

        /// <summary>
        /// Update custom stat with supplied value.
        /// </summary>
        void UpdateCustomStat(uint index, uint value);

        /// <summary>
        /// Send <see cref="IWritable"/> to <see cref="IPublicEventTeamMember"/>.
        /// </summary>
        void Send(IWritable message);

        /// <summary>
        /// Build <see cref="PublicEventStats"/> for <see cref="IPublicEventTeamMember"/>.
        /// </summary>
        PublicEventStats BuildStats();

        /// <summary>
        /// Build <see cref="PublicEventParticipantStats"/> for <see cref="IPublicEventTeamMember"/>.
        /// </summary>
        PublicEventParticipantStats BuildParticipantStats();
    }
}
