using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Game.Abstract.Event
{
    public interface IPublicEventStats : INetworkBuildable<PublicEventStats>
    {
        /// <summary>
        /// Update <see cref="Static.Event.PublicEventStat"/> with supplied value.
        /// </summary>
        void UpdateStat(Static.Event.PublicEventStat stat, uint value);

        /// <summary>
        /// Update custom stat with supplied value.
        /// </summary>
        void UpdateCustomStat(uint index, uint value);
    }
}
