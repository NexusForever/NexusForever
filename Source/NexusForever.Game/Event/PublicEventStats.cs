using NexusForever.Game.Abstract.Event;
using NetworkPublicEventStats = NexusForever.Network.World.Message.Model.Shared.PublicEventStats;

namespace NexusForever.Game.Event
{
    public class PublicEventStats : IPublicEventStats
    {
        private readonly Dictionary<Static.PublicEvent.PublicEventStat, uint> stats = [];
        private readonly Dictionary<uint, uint> customStats = [];

        /// <summary>
        /// Update <see cref="Static.PublicEvent.PublicEventStat"/> with supplied value.
        /// </summary>
        public void UpdateStat(Static.PublicEvent.PublicEventStat stat, uint value)
        {
            stats[stat] = value;
        }

        /// <summary>
        /// Update custom stat with supplied value.
        /// </summary>
        public void UpdateCustomStat(uint index, uint value)
        {
            customStats[index] = value;
        }

        public NetworkPublicEventStats Build()
        {
            var publicEventStats = new NetworkPublicEventStats();

            foreach ((Static.PublicEvent.PublicEventStat stat, uint value) in stats.OrderBy(e => e.Key))
            {
                publicEventStats.Mask.SetBit((uint)stat, true);
                publicEventStats.Values.Add(value);
            }

            foreach ((uint index, uint value) in customStats.OrderBy(e => e.Key))
            {
                publicEventStats.Mask.SetBit((uint)Static.PublicEvent.PublicEventStat.CustomStat00 + index, true);
                publicEventStats.Values.Add(value);
            }

            return publicEventStats;
        }
    }
}
