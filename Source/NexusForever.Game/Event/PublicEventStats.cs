using NexusForever.Game.Abstract.Event;
using NetworkPublicEventStats = NexusForever.Network.World.Message.Model.Shared.PublicEventStats;

namespace NexusForever.Game.Event
{
    public class PublicEventStats : IPublicEventStats
    {
        private readonly Dictionary<Static.Event.PublicEventStat, uint> stats = [];
        private readonly Dictionary<uint, uint> customStats = [];

        /// <summary>
        /// Update <see cref="Static.Event.PublicEventStat"/> with supplied value.
        /// </summary>
        public void UpdateStat(Static.Event.PublicEventStat stat, uint value)
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

            foreach ((Static.Event.PublicEventStat stat, uint value) in stats.OrderBy(e => e.Key))
            {
                publicEventStats.Mask.SetBit((uint)stat, true);
                publicEventStats.Values.Add(value);
            }

            foreach ((uint index, uint value) in customStats.OrderBy(e => e.Key))
            {
                publicEventStats.Mask.SetBit((uint)Static.Event.PublicEventStat.CustomStat + index, true);
                publicEventStats.Values.Add(value);
            }

            return publicEventStats;
        }
    }
}
