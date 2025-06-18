using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.PublicEvent
{
    public interface IPublicEventStats : INetworkBuildable<PublicEventStats>
    {
        /// <summary>
        /// Update <see cref="Static.PublicEvent.PublicEventStat"/> with supplied value.
        /// </summary>
        void UpdateStat(Static.PublicEvent.PublicEventStat stat, uint value);

        /// <summary>
        /// Update custom stat with supplied value.
        /// </summary>
        void UpdateCustomStat(uint index, uint value);
    }
}
