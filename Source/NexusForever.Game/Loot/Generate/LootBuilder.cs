using NexusForever.Game.Abstract.Loot.Generate;

namespace NexusForever.Game.Loot.Generate
{
    public class LootBuilder : ILootBuilder
    {
        private readonly List<ILootBuilderItem> items = [];

        public void AddLoot(ILootBuilderItem lootBuilderItem)
        {
            items.Add(lootBuilderItem);
        }

        public IEnumerable<ILootBuilderItem> GetItems()
        {
            return items;
        }
    }
}
