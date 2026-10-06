using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Loot;
using NexusForever.Game.Static.Loot;

namespace NexusForever.Game.Loot
{
    public class LootGroup : ILootGroup
    {
        public ulong Id { get; }
        public LootEntityType Type { get; }
        public float Probability { get; }

        private uint minDrop;
        private uint maxDrop;
        private LootConditionType conditionType;
        private uint condition;

        private List<ILootGroup> childLootGroups = new List<ILootGroup>();
        private List<ILootItem> lootItems = new List<ILootItem>();

        public LootGroup(LootGroupModel lootGroupModel)
        {
            Id              = lootGroupModel.Id;
            Probability     = lootGroupModel.Probability;
            minDrop         = lootGroupModel.MinDrop;
            maxDrop         = lootGroupModel.MaxDrop;
            if (minDrop > maxDrop)
                maxDrop = minDrop;
            conditionType   = (LootConditionType)lootGroupModel.ConditionType;
            condition       = lootGroupModel.Condition;

            foreach (LootGroupModel childLootGroup in DatabaseManager.Instance.GetDatabase<WorldDatabase>().GetLootGroupChildren(Id))
                childLootGroups.Add(new LootGroup(childLootGroup));

            foreach (LootItemModel lootItemModel in lootGroupModel.Item)
                lootItems.Add(new LootItem(lootItemModel));
        }

        /// <summary>
        /// Check this <see cref="LootGroup"/> to get a random chance of it dropping.
        /// </summary>
        private bool WillDrop(IPlayer player)
        {
            // TODO: Check Conditional
            if (!MeetsCondition(player))
                return false;

            double chance = new Random().NextDouble() * 100d;
            if (chance < Probability)
                return true;

            return false;
        }

        private bool MeetsCondition(IPlayer player)
        {
            if (conditionType == LootConditionType.None)
                return true;

            switch (conditionType)
            {
                /*case LootConditionType.QuestObjectiveActive:
                    return player.QuestManager.IsActiveObjectiveId(condition);*/
                default:
                    return true;
            }
        }

        /// <summary>
        /// Generate a randomised instance of <see cref="LootItem"/> and Count associated with this <see cref="LootGroup"/>, and all Child <see cref="LootGroup"/>.
        /// </summary>
        public Dictionary<ILootItem, uint> GenerateLootDrops(IPlayer player)
        {
            Dictionary<ILootItem, uint> itemsDropped = new Dictionary<ILootItem, uint>();

            if (!WillDrop(player))
                return itemsDropped;

            itemsDropped = GenerateLootItems(player);

            if (minDrop == 0u && maxDrop == 0u)
                return itemsDropped;

            int desiredDrop = new Random().Next((int)minDrop, (int)(maxDrop + 1));
            
            // If we don't have enough items, get some more.
            while (itemsDropped.Count < minDrop)
            {
                foreach ((ILootItem lootItem, uint count) in GenerateLootItems(player))
                    itemsDropped.Add(lootItem, count);
            }

            // If there are too many items, throw some out until we have the desired amount.
            if (itemsDropped.Count > desiredDrop)
            {
                Dictionary<ILootItem, uint> newItemsDropped = new Dictionary<ILootItem, uint>();
                while (itemsDropped.Count > maxDrop)
                {
                    int index = new Random().Next(0, itemsDropped.Count);
                    ILootItem item = itemsDropped.Keys.ElementAt(index);
                    uint count = itemsDropped.Values.ElementAt(index);

                    newItemsDropped.Add(item, count);
                    itemsDropped.Remove(item);
                }

                return newItemsDropped;
            }

            return itemsDropped;
        }

        /// <summary>
        /// Generate a <see cref="Dictionary{TKey, TValue}"/> containing <see cref="LootItem"/> and Counts for this <see cref="LootGroup"/>.
        /// </summary>
        private Dictionary<ILootItem, uint> GenerateLootItems(IPlayer player)
        {
            Dictionary<ILootItem, uint> itemsDropped = new Dictionary<ILootItem, uint>();

            foreach (ILootGroup lootGroup in childLootGroups)
                foreach ((ILootItem childGroupItem, uint count) in lootGroup.GenerateLootDrops(player))
                    itemsDropped.Add(childGroupItem, count);

            foreach (ILootItem lootItem in lootItems)
                if (lootItem.GetDrop(out uint count))
                    itemsDropped.Add(lootItem, count);

            return itemsDropped;
        }
    }
}
