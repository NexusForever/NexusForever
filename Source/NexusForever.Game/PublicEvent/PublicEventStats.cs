using NexusForever.Game.Abstract.PublicEvent;
using NexusForever.Game.Static.PublicEvent;
using NetworkPublicEventStats = NexusForever.Network.World.Message.Model.Shared.PublicEventStats;

namespace NexusForever.Game.PublicEvent
{
    public class PublicEventStats : IPublicEventStats
    {
        private static readonly Dictionary<PublicEventStat, uint> StatToBitDictionary = new()
        {
            { PublicEventStat.Damage,              0 },
            { PublicEventStat.Hits,                1 },
            { PublicEventStat.Haters,              2 },
            { PublicEventStat.Kills,               3 },
            { PublicEventStat.Deaths,              4 },
            { PublicEventStat.Healed,              5 },
            { PublicEventStat.Contributions,       6 },
            { PublicEventStat.DamageReceived,      7 },
            { PublicEventStat.HealingReceived,     8 },
            { PublicEventStat.Assists,             9 },
            { PublicEventStat.Saves,               10 },
            { PublicEventStat.Overhealed,          11 },
            { PublicEventStat.OverhealingReceived, 12 },
            { PublicEventStat.MedalPoints,         13 },
            { PublicEventStat.MaxMultiKill,        14 },
            { PublicEventStat.LongestImpulse,      15 },
            { PublicEventStat.LongestLife,         16 },
            { PublicEventStat.KillStreak,          17 },
            { PublicEventStat.CustomStat00,        18 },
            { PublicEventStat.CustomStat01,        19 },
            { PublicEventStat.CustomStat02,        20 },
            { PublicEventStat.CustomStat03,        21 },
            { PublicEventStat.CustomStat04,        22 },
            { PublicEventStat.CustomStat05,        23 }
        };

        private readonly Dictionary<PublicEventStat, uint> stats = [];
        private readonly Dictionary<uint, uint> customStats = [];

        /// <summary>
        /// Update <see cref="PublicEventStat"/> with supplied value.
        /// </summary>
        public void UpdateStat(PublicEventStat stat, uint value)
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

            foreach ((PublicEventStat stat, uint value) in stats.OrderBy(e => e.Key))
            {
                uint bitNumber = StatToBitDictionary[stat];

                publicEventStats.Mask.SetBit(bitNumber, true);
                publicEventStats.Values.Add(value);
            }

            foreach ((uint index, uint value) in customStats.OrderBy(e => e.Key))
            {
                uint bitNumber = StatToBitDictionary[(PublicEventStat)((uint)PublicEventStat.CustomStat00 + index)];

                publicEventStats.Mask.SetBit(bitNumber, true);
                publicEventStats.Values.Add(value);
            }

            return publicEventStats;
        }
    }
}
