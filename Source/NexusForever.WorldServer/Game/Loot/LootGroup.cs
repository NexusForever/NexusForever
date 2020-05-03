using NexusForever.Database.World.Model;
using NexusForever.Shared.Database;
using NexusForever.WorldServer.Game.Loot.Static;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusForever.WorldServer.Game.Loot
{
    public class LootGroup
    {
        public ulong Id { get; }
        public LootEntityType Type { get; }
        public float Probability { get; }

        private uint minDrop;
        private uint maxDrop;
        private uint condition;

        private List<LootGroup> childLootGroups = new List<LootGroup>();
        private List<LootItem> lootItems = new List<LootItem>();

        public LootGroup(LootGroupModel lootGroupModel)
        {
            Id = lootGroupModel.Id;
            Probability = lootGroupModel.Probability;
            minDrop = lootGroupModel.MinDrop;
            maxDrop = lootGroupModel.MaxDrop;
            if (minDrop > maxDrop)
                maxDrop = minDrop;
            condition = lootGroupModel.Condition;

            foreach (LootGroupModel childLootGroup in DatabaseManager.Instance.WorldDatabase.GetLootGroupChildren(Id))
                childLootGroups.Add(new LootGroup(childLootGroup));

            foreach (LootItemModel lootItemModel in lootGroupModel.Item)
                lootItems.Add(new LootItem(lootItemModel));
        }

        /// <summary>
        /// Check this <see cref="LootGroup"/> to get a random chance of it dropping.
        /// </summary>
        private bool WillDrop()
        {
            // TODO: Check Conditional

            double chance = new Random().NextDouble() * 100d;
            if (chance < Probability)
                return true;

            return false;
        }

        /// <summary>
        /// Generate a randomised instance of <see cref="LootItem"/> and Count associated with this <see cref="LootGroup"/>, and all Child <see cref="LootGroup"/>.
        /// </summary>
        public Dictionary<LootItem, uint> GenerateLootDrops()
        {
            Dictionary<LootItem, uint> itemsDropped = new Dictionary<LootItem, uint>();

            if (!WillDrop())
                return itemsDropped;

            itemsDropped = GenerateLootItems();

            if (minDrop == 0u && maxDrop == 0u)
                return itemsDropped;

            int desiredDrop = new Random().Next((int)minDrop, (int)(maxDrop + 1));
            
            // If we don't have enough items, get some more.
            while (itemsDropped.Count < minDrop)
            {
                foreach ((LootItem lootItem, uint count) in GenerateLootItems())
                    itemsDropped.Add(lootItem, count);
            }

            // If there are too many items, throw some out until we have the desired amount.
            if (itemsDropped.Count > desiredDrop)
            {
                Dictionary<LootItem, uint> newItemsDropped = new Dictionary<LootItem, uint>();
                while (itemsDropped.Count > maxDrop)
                {
                    int index = new Random().Next(0, itemsDropped.Count);
                    LootItem item = itemsDropped.Keys.ElementAt(index);
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
        private Dictionary<LootItem, uint> GenerateLootItems()
        {
            Dictionary<LootItem, uint> itemsDropped = new Dictionary<LootItem, uint>();

            foreach (LootGroup lootGroup in childLootGroups)
                foreach ((LootItem childGroupItem, uint count) in lootGroup.GenerateLootDrops())
                    itemsDropped.Add(childGroupItem, count);

            foreach (LootItem lootItem in lootItems)
                if (lootItem.GetDrop(out uint count))
                    itemsDropped.Add(lootItem, count);

            return itemsDropped;
        }
    }
}
