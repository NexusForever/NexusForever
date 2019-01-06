using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Inventory : IUpdate, IEnumerable<Bag>, ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly ulong characterId;
        private readonly Player player;
        private readonly Dictionary<InventoryLocation, Bag> bags = new Dictionary<InventoryLocation, Bag>();
        private readonly List<Item> deletedItems = new List<Item>();

        /// <summary>
        /// Create a new <see cref="Inventory"/> from <see cref="Player"/> database model.
        /// </summary>
        public Inventory(Player owner, Character model)
        {
            characterId = owner?.CharacterId ?? 0ul;
            player      = owner;

            foreach ((InventoryLocation location, uint defaultCapacity) in AssetManager.InventoryLocationCapacities)
                bags.Add(location, new Bag(location, defaultCapacity));

            foreach (var itemModel in model.Item)
                AddItem(new Item(itemModel));
        }

        /// <summary>
        /// Create a new <see cref="Inventory"/> from supplied <see cref="CharacterCreationEntry"/>.
        /// </summary>
        public Inventory(ulong owner, CharacterCreationEntry creationEntry)
        {
            characterId = owner;

            foreach ((InventoryLocation location, uint defaultCapacity) in AssetManager.InventoryLocationCapacities)
                bags.Add(location, new Bag(location, defaultCapacity));

            foreach (uint itemId in creationEntry.ItemIds.Where(i => i != 0u))
            {
                Item2Entry itemEntry = GameTableManager.Item.GetEntry(itemId);
                if (itemEntry == null)
                    throw new ArgumentNullException();

                Item2TypeEntry typeEntry = GameTableManager.ItemType.GetEntry(itemEntry.Item2TypeId);
                if (typeEntry.ItemSlotId == 0)
                    ItemCreate(itemEntry, 1u);
                else
                    ItemCreate(itemEntry);
            }
        }

        public void Update(double lastTick)
        {
            // TODO: tick items with limited lifespans
        }

        /// <summary>
        /// Returns <see cref="ItemVisual"/> for any visible items.
        /// </summary>
        public IEnumerable<ItemVisual> GetItemVisuals()
        {
            Bag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);

            foreach (Item item in bag)
            {
                ushort displayId = item.DisplayId;
                if (displayId == 0)
                    continue;

                Item2TypeEntry itemTypeEntry = GameTableManager.ItemType.GetEntry(item.Entry.Item2TypeId);
                yield return new ItemVisual
                {
                    Slot        = (ItemSlot)itemTypeEntry.ItemSlotId,
                    DisplayId   = displayId,
                     // TODO: send colour and dye data
                    ColourSetId = 0,
                    DyeData     = 0
                };
            }
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available <see cref="EquippedItem"/> bag index.
        /// </summary>
        public void ItemCreate(uint itemId)
        {
            Item2Entry itemEntry = GameTableManager.Item.GetEntry(itemId);
            if (itemEntry == null)
                throw new ArgumentNullException();

            ItemCreate(itemEntry);
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available <see cref="EquippedItem"/> bag index.
        /// </summary>
        public void ItemCreate(Item2Entry itemEntry)
        {
            if (itemEntry == null)
                throw new ArgumentNullException();

            Item2TypeEntry typeEntry = GameTableManager.ItemType.GetEntry(itemEntry.Item2TypeId);
            if (typeEntry.ItemSlotId == 0)
                throw new ArgumentException($"Item {itemEntry.Id} isn't equippable!");

            Bag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);

            // find first free bag index, some items can be equipped into multiple slots
            foreach (uint bagIndex in AssetManager.GetEquippedBagIndexes((ItemSlot)typeEntry.ItemSlotId))
            {
                if (bag.GetItem(bagIndex) != null)
                    continue;

                Item item = new Item(characterId, itemEntry);
                AddItem(item, InventoryLocation.Equipped, bagIndex);
                break;
            }
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(uint itemId, uint count, byte reason = 49)
        {
            Item2Entry itemEntry = GameTableManager.Item.GetEntry(itemId);
            if (itemEntry == null)
                throw new ArgumentNullException();

            ItemCreate(itemEntry, count, reason);
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(Item2Entry itemEntry, uint count, byte reason = 49)
        {
            if (itemEntry == null)
                throw new ArgumentNullException();

            Bag bag = GetBag(InventoryLocation.Inventory);
            Debug.Assert(bag != null);

            // update any existing stacks before creating new items
            if (itemEntry.MaxStackCount > 1)
            {
                foreach (Item item in bag.Where(i => i.Entry.Id == itemEntry.Id))
                {
                    uint stackCount = Math.Min(itemEntry.MaxStackCount - item.StackCount, itemEntry.MaxStackCount);
                    ItemStackCountUpdate(item, item.StackCount + stackCount);
                    count -= stackCount;
                }
            }

            // create new stacks for the remaining count
            while (count > 0)
            {
                uint bagIndex = bag.GetFirstAvailableBagIndex();
                if (bagIndex == uint.MaxValue)
                    return;

                var item = new Item(characterId, itemEntry, Math.Min(count, itemEntry.MaxStackCount));
                AddItem(item, InventoryLocation.Inventory, bagIndex);

                if (!player?.IsLoading ?? false)
                {
                    player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                    {
                        InventoryItem = new InventoryItem
                        {
                            Item = item.BuildNetworkItem(),
                            Reason = reason
                        }
                    });
                }

                count -= item.StackCount;
            }
        }

        /// <summary>
        /// Move <see cref="Item"/> from one <see cref="ItemLocation"/> to another, this is called directly from a packet hander.
        /// </summary>
        public void ItemMove(ItemLocation from, ItemLocation to)
        {
            Bag srcBag = GetBag(from.Location);
            if (srcBag == null)
                throw new InvalidPacketValueException();

            Item srcItem = srcBag.GetItem(from.BagIndex);
            if (srcItem == null)
                throw new InvalidPacketValueException();

            if (!IsValidLocationForItem(srcItem, to.Location, to.BagIndex))
                throw new InvalidPacketValueException();

            Bag dstBag = GetBag(to.Location);
            if (dstBag == null)
                throw new InvalidPacketValueException();

            Item dstItem = dstBag.GetItem(to.BagIndex);
            try
            {
                RemoveItem(srcItem);

                if (dstItem == null)
                {
                    // no item at destination, just a simple move
                    AddItem(srcItem, to.Location, to.BagIndex);

                    player.Session.EnqueueMessageEncrypted(new ServerItemMove
                    {
                        To = new ItemDragDrop
                        {
                            Guid     = srcItem.Guid,
                            DragDrop = ItemLocationToDragDropData(to.Location, (ushort)to.BagIndex)
                        }
                    });
                }
                else
                {
                    // item at destination, swap with source item
                    RemoveItem(dstItem);
                    AddItem(srcItem, to.Location, to.BagIndex);
                    AddItem(dstItem, from.Location, from.BagIndex);

                    player.Session.EnqueueMessageEncrypted(new ServerItemSwap
                    {
                        To = new ItemDragDrop
                        {
                            Guid     = srcItem.Guid,
                            DragDrop = ItemLocationToDragDropData(to.Location, (ushort)to.BagIndex)
                        },
                        From = new ItemDragDrop
                        {
                            Guid     = dstItem.Guid,
                            DragDrop = ItemLocationToDragDropData(from.Location, (ushort)from.BagIndex)
                        }
                    });
                }
            }
            catch (Exception exception)
            {
                // TODO: rollback
                log.Fatal(exception);
            }
        }

        public void ItemSplit()
        {
            // TODO
        }

        /// <summary>
        /// Delete <see cref="Item"/> at supplied <see cref="ItemLocation"/>, this is called directly from a packet hander.
        /// </summary>
        public void ItemDelete(ItemLocation from)
        {
            Bag srcBag = GetBag(from.Location);
            if (srcBag == null)
                throw new InvalidPacketValueException();

            Item srcItem = srcBag.GetItem(from.BagIndex);
            if (srcItem == null)
                throw new InvalidPacketValueException();

            srcBag.RemoveItem(srcItem);
            srcItem.EnqueueDelete();
            deletedItems.Add(srcItem);

            player.Session.EnqueueMessageEncrypted(new ServerItemDelete
            {
                Guid = srcItem.Guid
            });
        }

        /// <summary>
        /// Checks if supplied <see cref="InventoryLocation"/> and bag index valid for <see cref="Item"/>.
        /// </summary>
        private bool IsValidLocationForItem(Item item, InventoryLocation location, uint bagIndex)
        {
            Bag bag = GetBag(location);
            if (bag == null)
                return false;

            if (location == InventoryLocation.Equipped)
            {
                Item2TypeEntry typeEntry = GameTableManager.ItemType.GetEntry(item.Entry.Item2TypeId);
                if (typeEntry.ItemSlotId == 0)
                    return false;

                ImmutableList<EquippedItem> bagIndexes = AssetManager.GetEquippedBagIndexes((ItemSlot)typeEntry.ItemSlotId);
                if (bagIndexes.All(i => i != (EquippedItem) bagIndex))
                    return false;

                /*if (owner.Character.Class != item.Entry.ClassRequired)
                    return false;

                if (owner.Character.Race != item.Entry.RaceRequired)
                    return false;*/
            }

            return true;
        }

        /// <summary>
        /// Add <see cref="Item"/> to the supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        private void AddItem(Item item, InventoryLocation location, uint bagIndex)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location != InventoryLocation.None)
                throw new ArgumentException();

            item.Location = location;
            item.BagIndex = bagIndex;

            AddItem(item);
        }

        private void AddItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location == InventoryLocation.None)
                throw new ArgumentException();

            Bag bag = GetBag(item.Location);
            if (bag == null)
                throw new ArgumentNullException();

            bag.AddItem(item);

            if (bag.Location == InventoryLocation.Equipped)
                SendItemVisualUpdate(item);
        }

        /// <summary>
        /// Remove <see cref="Item"/> from it's current <see cref="InventoryLocation"/> and bag slot.
        /// </summary>
        private void RemoveItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location == InventoryLocation.None)
                throw new ArgumentException();

            Bag bag = GetBag(item.Location);
            Debug.Assert(bag != null);

            bag.RemoveItem(item);

            if (bag.Location == InventoryLocation.Equipped)
                SendItemVisualUpdate(item);
        }

        /// <summary>
        /// Broadcast <see cref="ServerItemVisualUpdate"/> for supplied <see cref="Item"/>, this will update the players look in the world.
        /// </summary>
        private void SendItemVisualUpdate(Item item)
        {
            if (item == null)
                throw new ArgumentNullException();

            Item2TypeEntry typeEntry = GameTableManager.ItemType.GetEntry(item.Entry.Item2TypeId);
            if (typeEntry.ItemSlotId == 0)
                throw new ArgumentException($"Item {item.Entry.Id} isn't equippable!");

            if (!player?.IsLoading ?? false)
            {
                bool visible = item.Location != InventoryLocation.None;
                player.EnqueueToVisible(new ServerItemVisualUpdate
                {
                    Guid        = player.Guid,
                    ItemVisuals = new List<ItemVisual>
                    {
                        new ItemVisual
                        {
                            Slot        = (ItemSlot)typeEntry.ItemSlotId,
                            DisplayId   = (ushort)(visible ? item.DisplayId : 0),
                            // TODO: send colour and dye data
                            ColourSetId = 0,
                            DyeData     = 0
                        }
                    }
                }, true);
            }
        }

        /// <summary>
        /// Update <see cref="Item"/> with supplied stack count.
        /// </summary>
        private void ItemStackCountUpdate(Item item, uint stackCount)
        {
            if (item == null)
                throw new ArgumentNullException();

            item.StackCount = stackCount;

            player.Session.EnqueueMessageEncrypted(new ServerItemStackCountUpdate
            {
                Guid       = item.Guid,
                StackCount = stackCount,
                Reason     = 0
            });
        }

        private Bag GetBag(InventoryLocation location)
        {
            return bags.TryGetValue(location, out Bag container) ? container : null;
        }

        private static ulong ItemLocationToDragDropData(InventoryLocation location, ushort slot)
        {
            // TODO: research this more, client version of this is more complex
            return ((ulong)location << 8) | slot;
        }

        public IEnumerator<Bag> GetEnumerator()
        {
            return bags.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Save(CharacterContext context)
        {
            foreach (Bag bag in bags.Values)
                foreach (Item item in bag)
                    item.Save(context);

            foreach (Item item in deletedItems)
                item.Save(context);
            deletedItems.Clear();
        }

        public Item GetItemByGuid(ulong guid)
        {
            foreach (Bag bag in bags.Values)
                foreach (Item item in bag)
                    if (item.Guid == guid)
                        return item;

            return null;
        }
    }
}
