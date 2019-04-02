using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Achievement.Static;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Inventory : IUpdate, IEnumerable<Bag>, ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static ulong ItemLocationToDragDropData(InventoryLocation location, ushort slot)
        {
            // TODO: research this more, client version of this is more complex
            return ((ulong)location << 8) | slot;
        }

        private readonly ulong characterId;
        private readonly Player player;
        private readonly Dictionary<InventoryLocation, Bag> bags = new();
        private readonly List<Item> deletedItems = new();

        static EquippedItem[] visualItemSlots = new EquippedItem[]
            {EquippedItem.Chest, EquippedItem.Head, EquippedItem.Legs, EquippedItem.Hands, EquippedItem.WeaponPrimary, EquippedItem.Shoulder, EquippedItem.Feet};
        static EquippedItem[] containerSlots = new EquippedItem[]
            {EquippedItem.Bag0, EquippedItem.Bag1, EquippedItem.Bag2, EquippedItem.Bag3};

        /// <summary>
        /// Create a new <see cref="Inventory"/> from <see cref="Player"/> database model.
        /// </summary>
        public Inventory(Player owner, CharacterModel model)
        {
            characterId = owner?.CharacterId ?? 0ul;
            player      = owner;

            foreach ((InventoryLocation location, uint defaultCapacity) in AssetManager.InventoryLocationCapacities)
                bags.Add(location, new Bag(location, defaultCapacity));

            foreach (var itemModel in model.Item.Select(i => i).OrderBy(i => i.Location).ToList())
            {
                Item item = new Item(itemModel);
                AddItem(item);
            }
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
                Item2Entry itemEntry = GameTableManager.Instance.Item.GetEntry(itemId);
                if (itemEntry == null)
                    throw new ArgumentNullException();

                Item2TypeEntry typeEntry = GameTableManager.Instance.ItemType.GetEntry(itemEntry.Item2TypeId);
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

        public void Save(CharacterContext context)
        {
            foreach (Item item in deletedItems)
                item.Save(context);
            
            deletedItems.Clear();

            foreach (Bag bag in bags.Values.Where(b => b.Location != InventoryLocation.Ability))
                bag.Save(context);
        }

        /// <summary>
        /// Returns <see cref="ItemVisual"/> for any visible items.
        /// </summary>
        public IEnumerable<ItemVisual> GetItemVisuals(Costume costume)
        {
            Bag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);

            foreach (Item item in bag)
            {
                if (!IsVisualItem((EquippedItem)item.BagIndex))
                    continue;

                Item2TypeEntry itemTypeEntry = GameTableManager.Instance.ItemType.GetEntry(item.Entry.Item2TypeId);

                ItemVisual visual = GetItemVisual((ItemSlot)itemTypeEntry.ItemSlotId, costume);
                if (visual != null)
                    yield return visual;
            }
        }

        /// <summary>
        /// Returns <see cref="ItemVisual"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        private ItemVisual GetItemVisual(ItemSlot itemSlot, Costume costume)
        {
            ImmutableList<EquippedItem> indexes = AssetManager.Instance.GetEquippedBagIndexes(itemSlot);
            if (indexes == null || indexes.Count != 1)
                throw new ArgumentOutOfRangeException();

            EquippedItem index = indexes[0];

            CostumeItem costumeItem = null;
            if (costume != null)
            {
                if (index == EquippedItem.WeaponPrimary)
                    costumeItem = costume.GetItem(CostumeItemSlot.Weapon);
                else if (index >= EquippedItem.Chest && index <= EquippedItem.Hands)
                {
                    // skip any slot that is hidden
                    if ((costume.Mask & (1 << (int)index)) == 0)
                        return new ItemVisual
                        {
                            Slot = itemSlot
                        };

                    costumeItem = costume.GetItem((CostumeItemSlot)index);
                }
            }

            Bag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);
            Item item = bag.GetItem((uint)index);

            return new ItemVisual
            {
                Slot      = itemSlot,
                DisplayId = Item.GetDisplayId(costumeItem?.Entry != null ? costumeItem.Entry : item?.Entry),
                DyeData   = costumeItem?.DyeData ?? 0
            };
        }

        /// <summary>
        /// Update <see cref="ItemVisual"/> and broadcast <see cref="ServerItemVisualUpdate"/> for optional supplied <see cref="Costume"/>.
        /// </summary>
        public void VisualUpdate(Costume costume)
        {
            var itemVisualUpdate = new ServerItemVisualUpdate
            {
                Guid = player.Guid
            };

            itemVisualUpdate.ItemVisuals.Add(VisualUpdate(ItemSlot.WeaponPrimary, costume));
            for (ItemSlot index = ItemSlot.ArmorChest; index <= ItemSlot.ArmorHands; index++)
                itemVisualUpdate.ItemVisuals.Add(VisualUpdate(index, costume));

            if (!player.IsLoading)
                player.EnqueueToVisible(itemVisualUpdate, true);
        }

        /// <summary>
        /// Update <see cref="ItemVisual"/> and broadcast <see cref="ServerItemVisualUpdate"/> for supplied <see cref="Item"/>
        /// </summary>
        private void VisualUpdate(Item item)
        {
            if (item == null)
                throw new ArgumentNullException();

            var itemVisualUpdate = new ServerItemVisualUpdate
            {
                Guid = player.Guid
            };

            Item2TypeEntry typeEntry = GameTableManager.Instance.ItemType.GetEntry(item.Entry.Item2TypeId);

            Costume costume = null;
            if (player.CostumeIndex >= 0)
                costume = player.CostumeManager.GetCostume((byte)player.CostumeIndex);

            itemVisualUpdate.ItemVisuals.Add(VisualUpdate((ItemSlot)typeEntry.ItemSlotId, costume));

            if (!player.IsLoading)
                player.EnqueueToVisible(itemVisualUpdate, true);
        }

        /// <summary>
        /// Update visual for supplied <see cref="ItemSlot"/> and optional <see cref="Costume"/>.
        /// </summary>
        private ItemVisual VisualUpdate(ItemSlot slot, Costume costume)
        {
            ItemVisual visual = GetItemVisual(slot, costume);
            player?.SetAppearance(visual);
            return visual;
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available <see cref="EquippedItem"/> bag index.
        /// </summary>
        public void ItemCreate(uint itemId)
        {
            Item2Entry itemEntry = GameTableManager.Instance.Item.GetEntry(itemId);
            if (itemEntry == null)
                throw new ArgumentNullException();

            ItemCreate(itemEntry);
        }

        /// <summary>
        /// Create a new <see cref="Item"/> from supplied <see cref="Spell4BaseEntry"/> in the first available <see cref="InventoryLocation.Ability"/> bag slot.
        /// </summary>
        public Item SpellCreate(Spell4BaseEntry spell4BaseEntry, ItemUpdateReason reason)
        {
            if (spell4BaseEntry == null)
                throw new ArgumentNullException();

            Bag bag = GetBag(InventoryLocation.Ability);
            Debug.Assert(bag != null);

            uint bagIndex = bag.GetFirstAvailableBagIndex();
            if (bagIndex == uint.MaxValue)
                return null;

            var spell = new Item(characterId, spell4BaseEntry);
            AddItem(spell, InventoryLocation.Ability, bagIndex);

            if (!player?.IsLoading ?? false)
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item   = spell.BuildNetworkItem(),
                        Reason = reason
                    }
                });
            }

            return spell;
        }

        /// <summary>
        /// Add <see cref="Item"/> in the first available bag index for the given <see cref="InventoryLocation"/> .
        /// </summary>
        public void AddItem(Item item, InventoryLocation inventoryLocation)
        {
            Bag bag = GetBag(inventoryLocation);
            uint bagIndex = bag.GetFirstAvailableBagIndex();

            if (bagIndex == uint.MaxValue)
            {
                throw new ArgumentException($"InventoryLocation {inventoryLocation} is full!");
            }
            
            // Stacks are bought back in full, so no need to worry about splitting stacks
            AddItem(item, inventoryLocation, bagIndex);

            if (!player?.IsLoading ?? false)
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item = item.BuildNetworkItem(),
                        Reason = ItemUpdateReason.NoReason
                    }
                });
            }
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available <see cref="EquippedItem"/> bag index.
        /// </summary>
        public void ItemCreate(Item2Entry itemEntry)
        {
            if (itemEntry == null)
                throw new ArgumentNullException();

            Item2TypeEntry typeEntry = GameTableManager.Instance.ItemType.GetEntry(itemEntry.Item2TypeId);
            if (typeEntry.ItemSlotId == 0)
                throw new ArgumentException($"Item {itemEntry.Id} isn't equippable!");

            Bag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);

            // find first free bag index, some items can be equipped into multiple slots
            foreach (uint bagIndex in AssetManager.Instance.GetEquippedBagIndexes((ItemSlot)typeEntry.ItemSlotId))
            {
                if (bag.GetItem(bagIndex) != null)
                    continue;

                var item = new Item(characterId, itemEntry);
                AddItem(item, InventoryLocation.Equipped, bagIndex);
                break;
            }
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(uint itemId, uint count, ItemUpdateReason reason = ItemUpdateReason.NoReason, uint charges = 0)
        {
            Item2Entry itemEntry = GameTableManager.Instance.Item.GetEntry(itemId);
            if (itemEntry == null)
                throw new ArgumentNullException();

            ItemCreate(itemEntry, count, reason, charges);
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(Item2Entry itemEntry, uint count, ItemUpdateReason reason = ItemUpdateReason.NoReason, uint charges = 0)
        {
            if (itemEntry == null)
                throw new ArgumentNullException();

            Bag bag = GetBag(InventoryLocation.Inventory);
            Debug.Assert(bag != null);

            // update any existing stacks before creating new items
            if (IsStackable(itemEntry) && itemEntry.MaxStackCount > 1)
            {
                foreach (Item item in bag.Where(i => i.Entry.Id == itemEntry.Id))
                {
                    if (count == 0u)
                        break;

                    if (item.StackCount == itemEntry.MaxStackCount)
                        continue;

                    uint newStackCount = Math.Min(item.StackCount + count, itemEntry.MaxStackCount);
                    count -= newStackCount - item.StackCount;
                    ItemStackCountUpdate(item, newStackCount, reason);
                }
            }

            // create new stacks for the remaining count
            while (count > 0)
            {
                uint bagIndex = bag.GetFirstAvailableBagIndex();
                if (bagIndex == uint.MaxValue)
                {
                    // If there is remaining count left, and this was created by SupplySatchelManager, then return the rest to the client.
                    if (count > 0 && reason == ItemUpdateReason.ResourceConversion)
                        player.SupplySatchelManager.AddAmount(new Item(characterId, itemEntry, count, charges), count);
                    else
                        player.Session.EnqueueMessageEncrypted(new ServerItemError
                        {
                            ErrorCode = ItemError.InventoryFull
                        });
                        
                    return;
                }

                var item = new Item(characterId, itemEntry, Math.Min(count, IsStackable(itemEntry) ? itemEntry.MaxStackCount : 1), charges);
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

            ItemMove(from, to, srcItem, dstItem);
        }

        private ItemError CheckValidItemForValidLocation(ItemLocation from, ItemLocation to, Item srcItem)
        {
            if (!IsEquippableBag(srcItem.Entry) && IsEquippedBagLocation(to))
                return ItemError.InvalidForThisSlot;

            if (IsEquippedBagLocation(from) && !IsEquippedBagLocation(to) && IsEquippableBag(srcItem.Entry))
                if (GetInventoryFreeBagIndexCount() < srcItem.Entry.MaxStackCount)
                    return ItemError.InventoryFull;

            return ItemError.IsValid;
        }

        /// <summary>
        /// Move <see cref="Item"/> from one <see cref="ItemLocation"/> to another, handling swapping if necessary.
        /// </summary>
        private void ItemMove(ItemLocation from, ItemLocation to, Item srcItem, Item dstItem = null)
        {
            if (srcItem == null || from == null || to == null)
                throw new InvalidPacketValueException();

            if (dstItem == null && GetBag(to.Location).GetItem(to.BagIndex) != null)
                throw new InvalidPacketValueException();

            ItemError validItemMove = CheckValidItemForValidLocation(from, to, srcItem);
            if (validItemMove != ItemError.IsValid)
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemError
                {
                    ItemGuid = srcItem.Guid,
                    ErrorCode = validItemMove
                });
                return;
            }

            try
            {
                if (dstItem == null)
                {
                    // no item at destination, just a simple move
                    RemoveItem(srcItem);
                    AddItem(srcItem, to.Location, to.BagIndex);

                    player.Session.EnqueueMessageEncrypted(new ServerItemMove
                    {
                        To = new ItemDragDrop
                        {
                            Guid = srcItem.Guid,
                            DragDrop = ItemLocationToDragDropData(to.Location, (ushort)to.BagIndex)
                        }
                    });
                }
                else if (srcItem.Entry.Id == dstItem.Entry.Id && IsStackable(dstItem.Entry))
                {
                    // item at destination with same entry, try and stack
                    uint newStackCount = Math.Min(dstItem.StackCount + srcItem.StackCount, dstItem.Entry.MaxStackCount);
                    uint oldStackCount = srcItem.StackCount - (newStackCount - dstItem.StackCount);
                    ItemStackCountUpdate(dstItem, newStackCount);

                    if (oldStackCount == 0u)
                    {
                        ItemDelete(new ItemLocation
                        {
                            Location = srcItem.Location,
                            BagIndex = srcItem.BagIndex
                        });
                    }
                    else
                        ItemStackCountUpdate(srcItem, oldStackCount);
                }
                else
                {
                    if (IsEquippedBagLocation(from) || IsEquippedBagLocation(to))
                    {
                        (int capacityChange, ItemError itemError) = GetCapacityChange(from, to, srcItem, dstItem);

                        if (itemError == ItemError.IsValid)
                        {
                            // item at destination, swap with source item
                            RemoveItem(srcItem, false);
                            RemoveItem(dstItem, false);
                            AddItem(srcItem, to.Location, to.BagIndex, false);
                            AddItem(dstItem, from.Location, from.BagIndex, false);

                            MoveItemsAfterBagSwap(capacityChange);

                            if (capacityChange != 0)
                                GetBag(InventoryLocation.Inventory).Resize(capacityChange);
                        }
                        else
                        {
                            player.Session.EnqueueMessageEncrypted(new ServerItemError
                            {
                                ItemGuid = srcItem.Guid,
                                ErrorCode = itemError
                            });
                            return;
                        }
                    }
                    else
                    {
                        // item at destination, swap with source item
                        RemoveItem(srcItem);
                        RemoveItem(dstItem);
                        AddItem(srcItem, to.Location, to.BagIndex);
                        AddItem(dstItem, from.Location, from.BagIndex);
                    }

                    player.Session.EnqueueMessageEncrypted(new ServerItemSwap
                    {
                        To = new ItemDragDrop
                        {
                            Guid = srcItem.Guid,
                            DragDrop = ItemLocationToDragDropData(to.Location, (ushort)to.BagIndex)
                        },
                        From = new ItemDragDrop
                        {
                            Guid = dstItem.Guid,
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

        private (int capacityChange, ItemError itemError) GetCapacityChange(ItemLocation from, ItemLocation to, Item srcItem, Item dstItem = null)
        {
            int capacityChange = 0;
            if (IsEquippableBag(srcItem.Entry) && IsEquippedBagLocation(to))
            {
                if (dstItem != null && !IsEquippedBagLocation(from))
                    capacityChange = (int)srcItem.Entry.MaxStackCount - (int)dstItem.Entry.MaxStackCount; // Replacing bag
            }
            else if (IsEquippableBag(srcItem.Entry) && IsEquippedBagLocation(from))
            {
                if (dstItem != null && IsEquippableBag(dstItem.Entry))
                    capacityChange = (int)dstItem.Entry.MaxStackCount - (int)srcItem.Entry.MaxStackCount; // Replacing bag
            }

            ItemError itemError = ItemError.IsValid;
            if (capacityChange < 0 && GetBag(InventoryLocation.Equipped).SlotsRemaining < (capacityChange * -1))
                itemError = ItemError.BagMustBeEmpty;

            return (capacityChange, itemError);
        }
        
        /// <summary>
        /// Split a subset of <see cref="Item"/> to create a new <see cref="Item"/> of split amount
        /// </summary>
        public void ItemSplit(ulong itemGuid, ItemLocation newItemLocation, uint count)
        {
            Item item = GetItem(itemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            if (IsEquippableBag(item.Entry))
                throw new InvalidPacketValueException();

            if (item.Entry.MaxStackCount <= 1u)
                throw new InvalidPacketValueException();

            if (count >= item.StackCount)
                throw new InvalidPacketValueException();

            Bag dstBag = GetBag(newItemLocation.Location);
            if (dstBag == null)
                throw new InvalidPacketValueException();

            Item dstItem = dstBag.GetItem(newItemLocation.BagIndex);
            if (dstItem != null)
                throw new InvalidPacketValueException();

            var newItem = new Item(characterId, item.Entry, Math.Min(count, item.Entry.MaxStackCount));
            AddItem(newItem, newItemLocation.Location, newItemLocation.BagIndex);

            if (!player?.IsLoading ?? false)
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item = newItem.BuildNetworkItem(),
                        Reason = ItemUpdateReason.NoReason
                    }
                });
            }

            ItemStackCountUpdate(item, item.StackCount - count);
        }

        /// <summary>
        /// Return <see cref="Item"/> at supplied <see cref="ItemLocation"/>.
        /// </summary>
        public Item GetItem(ItemLocation itemLocation)
        {
            return GetItem(itemLocation.Location, itemLocation.BagIndex);
        }

        /// <summary>
        /// Return <see cref="Item"/> at supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        public Item GetItem(InventoryLocation location, uint bagIndex)
        {
            Bag bag = GetBag(location);
            if (bag == null)
                throw new InvalidPacketValueException();

            Item item = bag.GetItem(bagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            return item;
        }

        /// <summary>
        /// Return <see cref="Item"/> with supplied guid.
        /// </summary>
        public Item GetItem(ulong guid)
        {
            foreach (Bag bag in bags.Values)
                foreach (Item item in bag)
                    if (item.Guid == guid)
                        return item;

            return null;
        }

        public Item GetSpell(Spell4BaseEntry spell4BaseEntry)
        {
            Bag bag = GetBag(InventoryLocation.Ability);
            foreach (Item item in bag)
                if (item.SpellEntry == spell4BaseEntry)
                    return item;

            return null;
        }

        /// <summary>
        /// Delete <see cref="Item"/> at supplied <see cref="ItemLocation"/>, this is called directly from a packet hander.
        /// </summary>
        public Item ItemDelete(ItemLocation from, ItemUpdateReason reason = ItemUpdateReason.Loot)
        {
            Bag srcBag = GetBag(from.Location);
            if (srcBag == null)
                throw new InvalidPacketValueException();

            Item srcItem = srcBag.GetItem(from.BagIndex);
            if (srcItem == null)
                throw new InvalidPacketValueException();

            return ItemDelete(srcBag, srcItem, reason);
        }

        private Item ItemDelete(Bag bag, Item item, ItemUpdateReason reason)
        {
            bag.RemoveItem(item);
            if (!item.PendingCreate)
            {
                item.EnqueueDelete();
                deletedItems.Add(item);
            }

            player.Session.EnqueueMessageEncrypted(new ServerItemDelete
            {
                Guid   = item.Guid,
                Reason = reason
            });

            return item;
        }

        /// <summary>
        /// Delete a supplied amount of an <see cref="Item"/>.
        /// </summary>
        public void ItemDelete(uint itemId, uint count = 1u)
        {
            Bag bag = GetBag(InventoryLocation.Inventory);
            foreach (Item item in bag.Where(i => i.Id == itemId))
            {
                if (item.StackCount > count)
                {
                    ItemStackCountUpdate(item, item.StackCount - count);
                    count = 0;
                }
                else
                {
                    ItemDelete(bag, item, ItemUpdateReason.Loot);
                    count -= item.StackCount;
                }

                if (count == 0)
                    break;
            }
        }

        /// <summary>
        /// Remove <see cref="Item"/> from this player's inventory without deleting the item from the DB
        /// </summary>
        public void ItemRemove(Item srcItem, ItemUpdateReason reason = ItemUpdateReason.NoReason)
        {
            if (srcItem == null)
                throw new InvalidPacketValueException("Item could not be found");

            Bag srcBag = GetBag(srcItem.Location);
            if (srcBag == null)
                throw new InvalidPacketValueException();

            // Intentionally not checking for bag resize because nothing should call this method without checking for non-equipped items first.

            srcBag.RemoveItem(srcItem);
            srcItem.CharacterId = null;

            player.Session.EnqueueMessageEncrypted(new ServerItemDelete
            {
                Guid = srcItem.Guid,
                Reason = reason
            });
        }

        /// <summary>
        /// Check if the <see cref="InventoryLocation.Inventory"/> for <see cref="Player"/> is full
        /// </summary>
        /// <returns></returns>
        public bool IsInventoryFull()
        {
            Bag bag = GetBag(InventoryLocation.Inventory);
            uint bagIndex = bag.GetFirstAvailableBagIndex();

            return bagIndex >= uint.MaxValue;
        }

        /// <summary>
        /// Return the amount of free bag indexes in <see cref="InventoryLocation.Inventory"/>.
        /// </summary>
        public uint GetInventoryFreeBagIndexCount()
        {
            Bag bag = GetBag(InventoryLocation.Inventory);
            return bag.GetFreeBagIndexCount();
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
                Item2TypeEntry typeEntry = GameTableManager.Instance.ItemType.GetEntry(item.Entry.Item2TypeId);
                if (typeEntry.ItemSlotId == 0)
                    return false;

                ImmutableList<EquippedItem> bagIndexes = AssetManager.Instance.GetEquippedBagIndexes((ItemSlot)typeEntry.ItemSlotId);
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
        private void AddItem(Item item, InventoryLocation location, uint bagIndex, bool resizeBags = true)
        {
            if (item == null)
                throw new ArgumentNullException();

            item.Location = location;
            item.BagIndex = bagIndex;

            AddItem(item, resizeBags);
        }

        public void AddItem(Item item, bool resizeBags = true)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location == InventoryLocation.None)
                throw new ArgumentException();

            Bag bag = GetBag(item.Location);
            if (bag == null)
                throw new ArgumentNullException();

            bag.AddItem(item);

            if (player != null && bag.Location == InventoryLocation.Equipped)
                if (IsVisualItem((EquippedItem)item.BagIndex))
                    VisualUpdate(item);

            if (resizeBags && IsEquippedBagLocation(item.Location, item.BagIndex))
                GetBag(InventoryLocation.Inventory).Resize((int)item.Entry.MaxStackCount);
        }

        /// <summary>
        /// Remove <see cref="Item"/> from it's current <see cref="InventoryLocation"/> and bag slot.
        /// </summary>
        private void RemoveItem(Item item, bool resizeBags = true)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location == InventoryLocation.None)
                throw new ArgumentException();

            Bag bag = GetBag(item.Location);
            Debug.Assert(bag != null);

            bag.RemoveItem(item);

            if (player != null && bag.Location == InventoryLocation.Equipped)
                if (IsVisualItem((EquippedItem)item.PreviousBagIndex)) // Using previous bag index because the item will've already been moved
                    VisualUpdate(item);

            if (resizeBags && IsEquippedBagLocation(item.PreviousLocation, item.PreviousBagIndex) && IsEquippableBag(item.Entry))
            {
                MoveItemsAfterBagSwap((int)(item.Entry.MaxStackCount * -1));
                GetBag(InventoryLocation.Inventory).Resize((int)item.Entry.MaxStackCount * -1);
            }
        }

        /// <summary>
        /// Update <see cref="Item"/> with supplied stack count.
        /// </summary>
        private void ItemStackCountUpdate(Item item, uint stackCount, ItemUpdateReason reason = ItemUpdateReason.NoReason)
        {
            if (item == null)
                throw new ArgumentNullException();

            item.StackCount = stackCount;

            player.Session.EnqueueMessageEncrypted(new ServerItemStackCountUpdate
            {
                Guid       = item.Guid,
                StackCount = stackCount,
                Reason     = reason
            });
        }

        /// <summary>
        /// Apply stack updates and deletion to <see cref="Item"/> on use
        /// </summary>
        public bool ItemUse(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Item is null.");

            // This should only apply for re-usable items, like Quest Clickies.
            if (item.Entry.MaxCharges == 0 && item.Entry.MaxStackCount == 1)
                return true;

            if ((item.Charges <= 0 && item.Entry.MaxCharges > 1)|| (item.StackCount <= 0 && item.Entry.MaxStackCount > 1))
                return false;

            player.AchievementManager.CheckAchievements(player, AchievementType.ItemConsume, item.Id);

            if(item.Charges >= 1 && item.Entry.MaxStackCount == 1)
                item.Charges--;

            if (item.Entry.MaxStackCount > 1 && item.StackCount > 0)
                ItemStackCountUpdate(item, item.StackCount - 1);

            // TODO: Set Deletion reason to 1, when consuming a single charge item.
            if ((item.StackCount == 0 && item.Entry.MaxStackCount > 1) || (item.Charges == 0 && item.Entry.MaxCharges > 0))
            {
                ItemDelete(new ItemLocation
                {
                    Location = item.Location,
                    BagIndex = item.BagIndex
                }, ItemUpdateReason.ConsumeCharge);
            }

            return true;
        }

        public void ItemMoveToSupplySatchel(Item item, uint amount)
        {
            if (player.SupplySatchelManager.IsFull(item))
                return;

            uint amountRemaining = player.SupplySatchelManager.AddAmount(item, amount);
            if (amountRemaining > 0)
                ItemStackCountUpdate(item, (item.StackCount - amount) + amountRemaining);
            else if (amount < item.StackCount)
                ItemStackCountUpdate(item, item.StackCount - amount);
            else
                ItemDelete(new ItemLocation 
                    {
                        Location = item.Location,
                        BagIndex = item.BagIndex
                    }, ItemUpdateReason.MaterialBagConversion);
        }
        
        /// Check if the current <see cref="Item"/> is in a visual slot
        /// </summary>
        private bool IsVisualItem(EquippedItem item)
        {
            return visualItemSlots.Contains(item);
        }

        /// <summary>
        /// Used to adjust items after a bag is equipped. Should only occur when the Inventory size shrinks.
        /// </summary>
        private void MoveItemsAfterBagSwap(int capacityChange)
        {
            if (capacityChange > 0)
                return;

            Bag inventory = GetBag(InventoryLocation.Inventory);
            if (inventory == null)
                throw new InvalidPacketValueException("Can't find Inventory.");

            int inventoryMaxIndex = inventory.GetSize() - 1;

            for (int i = inventoryMaxIndex; i > (inventoryMaxIndex + capacityChange); i--)
            {
                Item item = inventory.GetItem((uint)i);
                if (item != null)
                {
                    uint newItemIndex = inventory.GetFirstAvailableBagIndexAfterIndex((int)inventory.GetFirstAvailableBagIndex());
                    if (newItemIndex == uint.MaxValue)
                        throw new InvalidPacketValueException($"Missing BagIndex to move item {item.Guid} to.");

                    ItemMove(new ItemLocation
                    {
                        Location = item.Location,
                        BagIndex = item.BagIndex
                    },
                    new ItemLocation
                    {
                        Location = InventoryLocation.Inventory,
                        BagIndex = newItemIndex
                    },
                    item,
                    null);
                }
            }
        }

        /// <summary>
        /// Check if the <see cref="Item"/> is an equipped container
        /// </summary>
        private bool IsEquippedBag(Item item)
        {
            return containerSlots.Contains((EquippedItem)item.BagIndex);
        }

        /// <summary>
        /// Returns whether or not the provided <see cref="ItemLocation"/> is an Equipped Bag slot.
        /// </summary>
        private bool IsEquippedBagLocation(ItemLocation itemLocation)
        {
            return IsEquippedBagLocation(itemLocation.Location, itemLocation.BagIndex);
        }

        /// <summary>
        /// Returns whether or not the provided <see cref="InventoryLocation"/> and bagIndex match an Equipped Bag slot.
        /// </summary>
        private bool IsEquippedBagLocation(InventoryLocation inventoryLocation, uint bagIndex)
        {
            if (inventoryLocation != InventoryLocation.Equipped)
                return false;

            return containerSlots.Contains((EquippedItem)bagIndex);
        }

        // <summary>
        /// Returns whether this item is a stackable item
        /// </summary>
        public bool IsStackable(Item2Entry item2Entry)
        {
            // TODO: Figure out other non-stackable items, which have MaxStackCount > 1
            return !IsEquippableBag(item2Entry) && item2Entry.MaxStackCount > 1;
        }

        /// <summary>
        /// Returns whether this item is an equippable bag for expanding inventory slots
        /// </summary>
        public bool IsEquippableBag(Item2Entry item2Entry)
        {
            return item2Entry.Item2FamilyId == 5 && item2Entry.Item2CategoryId == 88 && item2Entry.Item2TypeId == 134;
        }

        private Bag GetBag(InventoryLocation location)
        {
            return bags.TryGetValue(location, out Bag container) ? container : null;
        }

        public IEnumerator<Bag> GetEnumerator()
        {
            return bags.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
