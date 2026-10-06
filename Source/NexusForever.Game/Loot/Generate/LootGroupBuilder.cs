using System.Collections.Immutable;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Loot.Generate;

namespace NexusForever.Game.Loot.Generate
{
    public class LootGroupBuilder : ILootGroupBuilder
    {
        private ImmutableList<LootGroupModel> lootGroups;
        private ImmutableDictionary<ulong, ImmutableList<LootGroupModel>> childLootGroups;

        public void Initialise(ImmutableList<LootGroupModel> lootGroups)
        {
            this.lootGroups = lootGroups;
            childLootGroups = lootGroups
                .Where(l => l.ParentId != null)
                .GroupBy(l => l.ParentId.Value)
                .ToImmutableDictionary(
                    g => g.Key,
                    g => g.ToImmutableList());
        }

        public IEnumerable<LootGroupModel> GetRootLootGroups()
        {
            return lootGroups.Where(l => l.ParentId == null);
        }

        public IEnumerable<LootGroupModel> GetLootGroupChildren(ulong id)
        {
            return childLootGroups.TryGetValue(id, out ImmutableList<LootGroupModel> lootGroups) ? lootGroups : [];
        }
    }
}
