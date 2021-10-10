using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using NexusForever.Shared;
using NexusForever.Shared.Database;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NLog;

namespace NexusForever.WorldServer.Game
{
    public class ItemManager : Singleton<ItemManager>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Id to be assigned to the next created item.
        /// </summary>
        public ulong NextItemId => nextItemId++;

        private ulong nextItemId;

        private ImmutableDictionary<uint, ItemInfo> item;
        private ImmutableDictionary<ItemSlot, ImmutableList<EquippedItem>> equippedItemSlots;

        private ItemManager()
        {
        }

        public void Initialise()
        {
            var sw = Stopwatch.StartNew();
            log.Info("Initialise item info...");

            nextItemId = DatabaseManager.Instance.CharacterDatabase.GetNextItemId() + 1ul;

            InitialiseItemInfo();
            InitialiseEquippedItemSlots();

            log.Info($"Cached {item.Count} items in {sw.ElapsedMilliseconds}ms.");
        }

        private void InitialiseItemInfo()
        {
            var builder = ImmutableDictionary.CreateBuilder<uint, ItemInfo>();
            foreach (Item2Entry entry in GameTableManager.Instance.Item.Entries)
            {
                var info = new ItemInfo(entry);
                builder.Add(info.Id, info);
            }

            item = builder.ToImmutable();
        }

        private void InitialiseEquippedItemSlots()
        {
            var builder = new Dictionary<ItemSlot, List<EquippedItem>>();
            foreach (ItemSlotEntry entry in GameTableManager.Instance.ItemSlot.Entries)
            {
                for (EquippedItem slot = EquippedItem.Chest; slot < EquippedItem.BankBag9; slot++)
                {
                    uint flags = 1u << (int)slot;
                    if ((entry.EquippedSlotFlags & flags) == 0)
                        continue;

                    if (!builder.TryGetValue((ItemSlot)entry.Id, out List<EquippedItem> equippedItems))
                    {
                        equippedItems = new List<EquippedItem>();
                        builder.Add((ItemSlot)entry.Id, equippedItems);
                    }

                    equippedItems.Add(slot);
                }
            }

            equippedItemSlots = builder.ToImmutableDictionary(e => e.Key, e => e.Value.ToImmutableList());
        }

        /// <summary>
        /// Return <see cref="ItemInfo"/> with supplied id.
        /// </summary>
        public ItemInfo GetItemInfo(uint id)
        {
            return item.TryGetValue(id, out ItemInfo info) ? info : null;
        }

        /// <summary>
        /// Returns a collection of bag indexes for <see cref="InventoryLocation.Equipped"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        public ImmutableList<EquippedItem> GetEquippedBagIndexes(ItemSlot slot)
        {
            return equippedItemSlots.TryGetValue(slot, out ImmutableList<EquippedItem> entries) ? entries : null;
        }
    }
}
