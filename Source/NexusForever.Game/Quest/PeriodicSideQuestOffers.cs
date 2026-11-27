using NexusForever.Game.Static.Quest;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Quest
{
    internal static class PeriodicSideQuestOffers
    {
        private sealed class Pool(uint id, uint offered, uint weight)
        {
            public uint Id { get; } = id;
            public uint Offered { get; } = offered;
            public uint Weight { get; } = weight;
            public ulong TotalWeight { get; private set; }
            public List<(ulong Threshold, Pool Child)> Children { get; } = [];

            public void Add(Pool child)
            {
                TotalWeight += child.Weight;
                Children.Add((TotalWeight, child));
            }
        }

        private static readonly Lazy<Pool[]> categories = new(BuildPools);
        private static readonly object sync = new();
        private static ulong cachedSeed;
        private static HashSet<uint> cachedOffers;

        // Keep one realm-wide offer per reset day, independently of reconnects.
        internal static ulong Seed(DateTime now) =>
            (ulong)(now.AddHours(-10).Ticks / TimeSpan.TicksPerDay);

        internal static bool IsOffered(Quest2Entry quest, ulong seed)
        {
            if (quest.QuestRepeatPeriodEnum is not ((uint)QuestRepeatPeriod.Daily or (uint)QuestRepeatPeriod.Weekly) ||
                quest.PeriodicQuestGroupId == 0u || quest.PeriodicQuestWeight == 0u ||
                GameTableManager.Instance.PeriodicQuestGroup?.GetEntry(quest.PeriodicQuestGroupId) is not { ContractTypeEnum: 0u } group ||
                GameTableManager.Instance.PeriodicQuestSet.GetEntry(group.PeriodicQuestSetId) is not { } set ||
                GameTableManager.Instance.PeriodicQuestSetCategory.GetEntry(set.PeriodicQuestSetCategoryId) == null)
                return true;

            return OfferedForSeed(seed).Contains(quest.Id);
        }

        internal static IReadOnlySet<uint> OfferedForSeed(ulong seed)
        {
            lock (sync)
            {
                if (cachedOffers == null || cachedSeed != seed)
                {
                    cachedOffers = categories.Value.SelectMany(category => Draw(category, (uint)seed))
                        .SelectMany(set => Draw(set, (uint)seed))
                        .SelectMany(group => Draw(group, (uint)seed))
                        .Select(quest => quest.Id).ToHashSet();
                    cachedSeed = seed;
                }

                return cachedOffers;
            }
        }

        private static Pool[] BuildPools()
        {
            var tables = GameTableManager.Instance;
            var byCategory = new SortedDictionary<uint, Pool>();
            var bySet = new Dictionary<uint, Pool>();
            var byGroup = new Dictionary<uint, Pool>();

            // The client inserts groups and quests in their original table order.
            foreach (PeriodicQuestGroupEntry group in tables.PeriodicQuestGroup.Entries)
            {
                PeriodicQuestSetEntry set = tables.PeriodicQuestSet.GetEntry(group.PeriodicQuestSetId);
                if (set == null)
                    continue;

                if (!bySet.TryGetValue(set.Id, out Pool setPool))
                {
                    if (!byCategory.TryGetValue(set.PeriodicQuestSetCategoryId, out Pool categoryPool))
                    {
                        PeriodicQuestSetCategoryEntry category = tables.PeriodicQuestSetCategory.GetEntry(set.PeriodicQuestSetCategoryId);
                        if (category == null)
                            continue;

                        categoryPool = new Pool(category.Id, category.PeriodicSetsOffered, 0u);
                        byCategory.Add(category.Id, categoryPool);
                    }

                    setPool = new Pool(set.Id, set.PeriodicGroupsOffered, set.Weight);
                    bySet.Add(set.Id, setPool);
                    categoryPool.Add(setPool);
                }

                var groupPool = new Pool(group.Id, group.PeriodicQuestsOffered, group.Weight);
                byGroup.Add(group.Id, groupPool);
                setPool.Add(groupPool);
            }

            foreach (Quest2Entry entry in tables.Quest2.Entries)
            {
                if (entry.QuestRepeatPeriodEnum == 0u || entry.PeriodicQuestWeight == 0u ||
                    (!byGroup.TryGetValue(entry.PeriodicQuestGroupId, out Pool group) &&
                        !byGroup.TryGetValue(1u, out group)))
                    continue;

                group.Add(new Pool(entry.Id, 0u, entry.PeriodicQuestWeight));
            }

            return byCategory.Values.ToArray();
        }

        private static IEnumerable<Pool> Draw(Pool pool, uint seed)
        {
            if (pool.Offered >= pool.Children.Count)
                return pool.Children.Select(entry => entry.Child);

            var left = pool.Children.ToList();
            var chosen = new List<Pool>();
            ulong total = pool.TotalWeight;

            // Each pick has its own MT19937 seed and removes the selected weight.
            for (uint remaining = pool.Offered; remaining > 0; remaining--)
            {
                ulong roll = total <= 1 ? 1ul :
                    1ul + (ulong)(FirstMtValue(unchecked(pool.Id + remaining + seed)) * (1d / 4294967296d) * total);
                int index = left.FindIndex(entry => entry.Threshold >= roll);
                if (index < 0)
                    return [];

                Pool child = left[index].Child;
                chosen.Add(child);
                left.RemoveAt(index);
                total -= child.Weight;
                for (int i = index; i < left.Count; i++)
                    left[i] = (left[i].Threshold - child.Weight, left[i].Child);
            }

            return chosen;
        }

        private static uint FirstMtValue(uint seed)
        {
            // Only the first output of each independently seeded generator is used.
            uint at0 = seed;
            uint at1 = unchecked(1812433253u * (seed ^ (seed >> 30)) + 1u);
            uint state = at1;
            for (uint i = 2; i <= 397; i++)
                state = unchecked(1812433253u * (state ^ (state >> 30)) + i);

            uint pair = (at0 & 0x80000000u) | (at1 & 0x7fffffffu);
            uint value = state ^ (pair >> 1) ^ ((pair & 1u) == 0u ? 0u : 0x9908b0dfu);
            value ^= value >> 11;
            value ^= (value << 7) & 0x9d2c5680u;
            value ^= (value << 15) & 0xefc60000u;
            return value ^ (value >> 18);
        }
    }
}
