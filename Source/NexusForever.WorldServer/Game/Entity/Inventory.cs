﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Shared;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Game.Achievement.Static;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Static;
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

        /// <summary>
        /// Create a new <see cref="Inventory"/> from <see cref="Player"/> database model.
        /// </summary>
        public Inventory(Player owner, CharacterModel model)
        {
            characterId = owner?.CharacterId ?? 0ul;
            player      = owner;

            foreach ((InventoryLocation location, uint defaultCapacity) in AssetManager.InventoryLocationCapacities)
                bags.Add(location, new Bag(location, defaultCapacity));

            foreach (ItemModel itemModel in model.Item
                .Select(i => i)
                .OrderBy(i => i.Location))
            {
                var item = new Item(itemModel);
                AddItem(item, (InventoryLocation)itemModel.Location, itemModel.BagIndex);
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
                ItemInfo info = ItemManager.Instance.GetItemInfo(itemId);
                if (info == null)
                    throw new ArgumentNullException();

                ItemCreate(info.IsEquippable() ? InventoryLocation.Equipped : InventoryLocation.Inventory, info, 1u);
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
        /// Returns if <see cref="InventoryLocation"/> and bag index is a visible item slot.
        /// </summary>
        public bool IsVisualItemSlot(InventoryLocation location, uint bagIndex)
        {
            return location == InventoryLocation.Equipped
                && (EquippedItem)bagIndex is EquippedItem.Chest or EquippedItem.Head or EquippedItem.Legs or EquippedItem.Hands
                    or EquippedItem.WeaponPrimary or EquippedItem.Shoulder or EquippedItem.Feet;
        }

        /// <summary>
        /// Returns if <see cref="InventoryLocation"/> and bag index is a bag slot.
        /// </summary>
        public bool IsEquippableBagSlot(InventoryLocation location, uint bagIndex)
        {
            return location == InventoryLocation.Equipped
                   && (EquippedItem) bagIndex is EquippedItem.Bag0 or EquippedItem.Bag1 or EquippedItem.Bag2 or EquippedItem.Bag3;
        }

        /// <summary>
        /// Returns if <see cref="InventoryLocation"/> and bag index is a bank bag slot.
        /// </summary>
        public bool IsEquippableBankBagSlot(InventoryLocation location, uint bagIndex)
        {
            return location == InventoryLocation.Equipped
                && (EquippedItem) bagIndex is EquippedItem.BankBag0 or EquippedItem.BankBag1 or EquippedItem.BankBag2
                    or EquippedItem.BankBag3 or EquippedItem.BankBag4 or EquippedItem.BankBag5 or EquippedItem.BankBag6
                    or EquippedItem.BankBag7 or EquippedItem.BankBag8 or EquippedItem.BankBag9;
        }

        /// <summary>
        /// Return if <see cref="InventoryLocation"/> has no free bag indexes remaining.
        /// </summary>
        public bool IsInventoryFull(InventoryLocation location)
        {
            return GetInventorySlotsRemaining(location) == 0u;
        }

        /// <summary>
        /// Return the amount of free bag indexes in <see cref="InventoryLocation.Inventory"/>.
        /// </summary>
        public uint GetInventorySlotsRemaining(InventoryLocation location)
        {
            Bag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            return bag.SlotsRemaining;
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
                throw new ArgumentException();

            Item item = bag.GetItem(bagIndex);
            if (item == null)
                throw new ArgumentException();

            return item;
        }

        /// <summary>
        /// Return <see cref="Item"/> with supplied guid.
        /// </summary>
        public Item GetItem(ulong guid)
        {
            return bags.Values
                .SelectMany(b => b)
                .FirstOrDefault(i => i.Guid == guid);
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
                if (!IsVisualItemSlot(item.Location, item.BagIndex))
                    continue;

                ItemVisual visual = GetItemVisual((ItemSlot)item.Info.TypeEntry.ItemSlotId, costume);
                if (visual != null)
                    yield return visual;
            }
        }

        /// <summary>
        /// Returns <see cref="ItemVisual"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        private ItemVisual GetItemVisual(ItemSlot itemSlot, Costume costume)
        {
            ImmutableList<EquippedItem> indexes = ItemManager.Instance.GetEquippedBagIndexes(itemSlot);
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
                DisplayId = Item.GetDisplayId(costumeItem?.Entry ?? item?.Info.Entry),
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

            Costume costume = null;
            if (player.CostumeIndex >= 0)
                costume = player.CostumeManager.GetCostume((byte)player.CostumeIndex);

            itemVisualUpdate.ItemVisuals.Add(VisualUpdate((ItemSlot)item.Info.TypeEntry.ItemSlotId, costume));

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
        /// Create a new <see cref="Item"/> from supplied <see cref="Spell4BaseEntry"/> in the first available <see cref="InventoryLocation.Ability"/> bag slot.
        /// </summary>
        public Item SpellCreate(Spell4BaseEntry spell4BaseEntry, ItemUpdateReason reason = ItemUpdateReason.NoReason)
        {
            if (spell4BaseEntry == null)
                throw new ArgumentNullException();

            var spell = new Item(characterId, spell4BaseEntry);
            AddItem(spell, InventoryLocation.Ability, reason);

            return spell;
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(InventoryLocation location, uint itemId, uint count, ItemUpdateReason reason = ItemUpdateReason.NoReason, uint charges = 0)
        {
            ItemInfo info = ItemManager.Instance.GetItemInfo(itemId);
            if (info == null)
                throw new ArgumentNullException();

            ItemCreate(location, info, count, reason, charges);
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(InventoryLocation location, ItemInfo info, uint count, ItemUpdateReason reason = ItemUpdateReason.NoReason, uint charges = 0)
        {
            if (info == null)
                throw new ArgumentNullException();

            Bag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            // update any existing stacks before creating new items
            if (info.IsStackable())
            {
                foreach (Item item in bag.Where(i => i.Info.Id == info.Id))
                {
                    if (count == 0u)
                        break;

                    if (item.StackCount == info.Entry.MaxStackCount)
                        continue;

                    uint newStackCount = Math.Min(item.StackCount + count, info.Entry.MaxStackCount);
                    count -= newStackCount - item.StackCount;
                    ItemStackCountUpdate(item, newStackCount, reason);
                }
            }

            // create new stacks for the remaining count
            while (count > 0)
            {
                uint? bagIndex = location == InventoryLocation.Equipped ? bag.GetFirstAvailableBagIndex((ItemSlot)info.SlotEntry.Id) : bag.GetFirstAvailableBagIndex();
                if (!bagIndex.HasValue)
                {
                    // If there is remaining count left, and this was created by SupplySatchelManager, then return the rest to the client.
                    if (count > 0 && reason == ItemUpdateReason.ResourceConversion)
                        player.SupplySatchelManager.AddAmount(new Item(characterId, info, count, charges), count);
                    else
                    {
                        player.Session.EnqueueMessageEncrypted(new ServerItemError
                        {
                            ErrorCode = GenericError.ItemInventoryFull
                        });
                    }

                    return;
                }

                var item = new Item(characterId, info, Math.Min(count, info.IsStackable() ? info.Entry.MaxStackCount : 1), charges);
                AddItem(item, location, bagIndex.Value);

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
        /// Returns if <see cref="Item"/> can be moved to supplied <see cref="ItemLocation"/>.
        /// </summary>
        public GenericError? CanMoveItem(Item item, ItemLocation location)
        {
            return CanMoveItem(item, location.Location, location.BagIndex);
        }

        /// <summary>
        /// Returns if <see cref="Item"/> can be moved to supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        public GenericError? CanMoveItem(Item item, InventoryLocation location, uint bagIndex)
        {
            // ItemInventoryFull
            // ItemUnique
            // ItemNotValidForSlot
            // ItemBagMustBeEmpty
            // ItemWrongRace
            // ItemWrongClass
            // ItemLevelToLow
            // PlayerCannotWhileInCombat
            // ItemCannotBeSalvaged
            // ItemWrongFaction
            // ItemCannotBeDeleted

            Bag bag = GetBag(location);
            if (bag == null)
                return GenericError.ItemNotValidForSlot;

            if (bag.Slots < bagIndex + 1)
                return GenericError.ItemNotValidForSlot;

            if (IsInventoryFull(location))
                return GenericError.ItemInventoryFull;

            if (location == InventoryLocation.Inventory)
            {
                // when removing bag capacity, make sure there is enough room left for items in the inventory
                if (item.Info.IsEquippableBag()
                    && IsEquippableBagSlot(item.Location, item.BagIndex)
                    && GetInventorySlotsRemaining(InventoryLocation.Inventory) < item.Info.Entry.MaxStackCount)
                    return GenericError.ItemBagMustBeEmpty;
            }
            else if (location == InventoryLocation.Equipped)
            {
                if (!item.Info.IsEquippable())
                    return GenericError.ItemNotValidForSlot;

                if (!item.Info.IsEquippableIntoSlot((EquippedItem)bagIndex))
                    return GenericError.ItemNotValidForSlot;

                /*if (owner.Character.Class != item.Entry.ClassRequired)
			        return GenericError.ItemWrongClass;

		        if (owner.Character.Race != item.Entry.RaceRequired)
			        return GenericError.ItemWrongRace;*/
            }

            return null;
        }

        /// <summary>
        /// Move <see cref="Item"/> to supplied <see cref="ItemLocation"/>.
        /// </summary>
        public void ItemMove(Item item, ItemLocation location)
        {
            ItemMove(item, location.Location, location.BagIndex);
        }

        /// <summary>
        /// Move <see cref="Item"/> to supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        public void ItemMove(Item item, InventoryLocation location, uint bagIndex)
        {
            if (item == null)
                throw new ArgumentNullException();

            Bag bag = GetBag(item.Location);
            if (bag == null)
                throw new ArgumentException();

            Bag dstBag = GetBag(location);
            if (dstBag == null)
                throw new ArgumentException();

            Item dstItem = dstBag.GetItem(bagIndex);

            try
            {
                if (dstItem == null)
                {
                    // item move is between the same bag, just a simple move
                    if (bag.Location == dstBag.Location)
                        bag.MoveItem(item, bagIndex);
                    else
                    {
                        RemoveItem(item);

                        // ensure slot is still valid
                        // this can happen when removing a bag to a slot that was removed during the resize
                        if (dstBag.Slots < bagIndex - 1)
                        {
                            uint? newBagIndex = dstBag.GetFirstAvailableBagIndex();
                            if (!newBagIndex.HasValue)
                                throw new InvalidOperationException();

                            bagIndex = newBagIndex.Value;
                        }

                        AddItem(item, location, bagIndex);
                    }

                    player.Session.EnqueueMessageEncrypted(new ServerItemMove
                    {
                        To = new ItemDragDrop
                        {
                            Guid     = item.Guid,
                            DragDrop = ItemLocationToDragDropData(item.Location, (ushort)item.BagIndex)
                        }
                    });
                }
                else
                {
                    // item at destination with same entry, try and stack
                    if (dstItem.Info.IsStackable()
                        && item.Info.Entry.Id == dstItem.Info.Entry.Id)
                    {
                        uint newStackCount = Math.Min(dstItem.StackCount + item.StackCount, dstItem.Info.Entry.MaxStackCount);
                        uint oldStackCount = item.StackCount - (newStackCount - dstItem.StackCount);
                        ItemStackCountUpdate(dstItem, newStackCount);

                        if (oldStackCount == 0u)
                        {
                            ItemDelete(new ItemLocation
                            {
                                Location = item.Location,
                                BagIndex = item.BagIndex
                            });
                        }
                        else
                            ItemStackCountUpdate(item, oldStackCount);
                    }
                    // item swap is between the same bag, just a simple move
                    else if (bag.Location == dstBag.Location)
                        bag.SwapItem(item, dstItem);
                    // items being swapped are 2 bags, we need to calculate the difference between the two
                    else if (item.Info.IsEquippableBag() && dstItem.Info.IsEquippableBag())
                    {
                        bag.RemoveItem(item);
                        dstBag.RemoveItem(dstItem);
                        bag.AddItem(dstItem, item.PreviousBagIndex);
                        dstBag.AddItem(item, dstItem.PreviousBagIndex);

                        int capacityChange = (int)item.Info.Entry.MaxStackCount - (int)dstItem.Info.Entry.MaxStackCount;
                        if (capacityChange != 0)
                        {
                            if (IsEquippableBagSlot(item.Location, item.BagIndex))
                                InventoryResize(InventoryLocation.Inventory, capacityChange);
                            if (IsEquippableBankBagSlot(item.Location, item.BagIndex))
                                InventoryResize(InventoryLocation.PlayerBank, capacityChange);
                        }
                    }
                    else
                    {
                        RemoveItem(item);
                        RemoveItem(dstItem);
                        AddItem(item, dstItem.PreviousLocation, dstItem.PreviousBagIndex);
                        AddItem(dstItem, item.PreviousLocation, item.PreviousBagIndex);
                    }

                    player.Session.EnqueueMessageEncrypted(new ServerItemSwap
                    {
                        To = new ItemDragDrop
                        {
                            Guid     = item.Guid,
                            DragDrop = ItemLocationToDragDropData(item.Location, (ushort)item.BagIndex)
                        },
                        From = new ItemDragDrop
                        {
                            Guid     = dstItem.Guid,
                            DragDrop = ItemLocationToDragDropData(dstItem.Location, (ushort)dstItem.BagIndex)
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

        /// <summary>
        /// Split a subset of <see cref="Item"/> to create a new <see cref="Item"/> of split amount
        /// </summary>
        public void ItemSplit(ulong itemGuid, ItemLocation newItemLocation, uint count)
        {
            Item item = GetItem(itemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            if (item.Info.IsEquippableBag())
                throw new InvalidPacketValueException();

            if (item.Info.Entry.MaxStackCount <= 1u)
                throw new InvalidPacketValueException();

            if (count >= item.StackCount)
                throw new InvalidPacketValueException();

            Bag dstBag = GetBag(newItemLocation.Location);
            if (dstBag == null)
                throw new InvalidPacketValueException();

            Item dstItem = dstBag.GetItem(newItemLocation.BagIndex);
            if (dstItem != null)
                throw new InvalidPacketValueException();

            var newItem = new Item(characterId, item.Info, Math.Min(count, item.Info.Entry.MaxStackCount));
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
        public void ItemRemove(Item item, ItemUpdateReason reason = ItemUpdateReason.NoReason)
        {
            if (item == null)
                throw new ArgumentNullException();

            Bag srcBag = GetBag(item.Location);
            if (srcBag == null)
                throw new ArgumentException();

            RemoveItem(item);
            item.CharacterId = null;

            player.Session.EnqueueMessageEncrypted(new ServerItemDelete
            {
                Guid   = item.Guid,
                Reason = reason
            });
        }

        /// <summary>
        /// Add <see cref="Item"/> in the first available bag index for the given <see cref="InventoryLocation"/> .
        /// </summary>
        public void AddItem(Item item, InventoryLocation location, ItemUpdateReason reason = ItemUpdateReason.NoReason)
        {
            Bag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            uint? bagIndex = location == InventoryLocation.Equipped ? bag.GetFirstAvailableBagIndex((ItemSlot)item.Info.SlotEntry.Id) : bag.GetFirstAvailableBagIndex();
            if (!bagIndex.HasValue)
                throw new ArgumentException();

            // Stacks are bought back in full, so no need to worry about splitting stacks
            AddItem(item, location, bagIndex.Value);

            if (!player?.IsLoading ?? false)
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item   = item.BuildNetworkItem(),
                        Reason = reason
                    }
                });
            }
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

            Bag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            bag.AddItem(item, bagIndex);

            if (IsVisualItemSlot(item.Location, item.BagIndex) && player != null)
                VisualUpdate(item);

            if (IsEquippableBagSlot(item.Location, item.BagIndex))
                InventoryResize(InventoryLocation.Inventory, (int)item.Info.Entry.MaxStackCount);
            if (IsEquippableBankBagSlot(item.Location, item.BagIndex))
                InventoryResize(InventoryLocation.PlayerBank, (int)item.Info.Entry.MaxStackCount);
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
            if (bag == null)
                throw new ArgumentException();

            if (IsVisualItemSlot(item.Location, item.BagIndex) && player != null)
                VisualUpdate(item);

            if (IsEquippableBagSlot(item.Location, item.BagIndex) && item.Info.IsEquippableBag())
                InventoryResize(InventoryLocation.Inventory, (int)-item.Info.Entry.MaxStackCount);
            if (IsEquippableBankBagSlot(item.Location, item.BagIndex) && item.Info.IsEquippableBag())
                InventoryResize(InventoryLocation.PlayerBank, (int)-item.Info.Entry.MaxStackCount);

            bag.RemoveItem(item);
        }

        /// <summary>
        /// Resize <see cref="InventoryLocation"/> with supplied capacity change.
        /// </summary>
        private void InventoryResize(InventoryLocation location, int capacityChange)
        {
            Bag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            if (capacityChange < 0)
            {
                for (uint bagIndex = bag.Slots - 1; bagIndex >= (bag.Slots + capacityChange); bagIndex--)
                {
                    Item item = bag.GetItem(bagIndex);
                    if (item == null)
                        continue;

                    uint? newBagIndex = bag.GetFirstAvailableBagIndex();
                    if (!newBagIndex.HasValue)
                        throw new InvalidOperationException();

                    ItemMove(item, location, newBagIndex.Value);
                    //bag.MoveItem(item, newBagIndex.Value);
                }
            }

            bag.Resize(capacityChange);
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
            if (item.Info.Entry.MaxCharges == 0 && item.Info.Entry.MaxStackCount == 1)
                return true;

            if ((item.Charges <= 0 && item.Info.Entry.MaxCharges > 1)|| (item.StackCount <= 0 && item.Info.Entry.MaxStackCount > 1))
                return false;

            player.AchievementManager.CheckAchievements(player, AchievementType.ItemConsume, item.Id);

            if(item.Charges >= 1 && item.Info.Entry.MaxStackCount == 1)
                item.Charges--;

            if (item.Info.Entry.MaxStackCount > 1 && item.StackCount > 0)
                ItemStackCountUpdate(item, item.StackCount - 1);

            // TODO: Set Deletion reason to 1, when consuming a single charge item.
            if ((item.StackCount == 0 && item.Info.Entry.MaxStackCount > 1) || (item.Charges == 0 && item.Info.Entry.MaxCharges > 0))
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

        /// <summary>
        /// Returns whether or not this <see cref="Player"/> has an item with the supplied Item2Id
        /// </summary>
        public bool HasItem(uint item2Id)
        {
            foreach (Bag bag in bags.Values)
            {
                if (bag.Location == InventoryLocation.Ability)
                    continue;

                foreach (Item item in bag)
                {
                    if (item.Info != null && item.Info.Entry?.Id == item2Id) 
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Used to unlock the extra rune socket for an <see cref="Item"/>.
        /// </summary>
        public void RuneSlotUnlock(ulong itemGuid, RuneType newType, bool useServiceTokens)
        {
            Item item = GetItem(itemGuid);
            if (item == null)
                throw new ArgumentException();

            if (item.UnlockRuneSlot(newType))
            {
                // TODO: Calculate Cost

                SendItemModify(item);
            }
            else
                player.Session.EnqueueMessageEncrypted(new ServerItemError
                {
                    ItemGuid = itemGuid,
                    ErrorCode = GenericError.UnlockItemFailed
                });
        }
        
        /// <summary>
        /// USed to reroll a rune socket for an <see cref="Item"/>.
        /// </summary>
        public void RuneSlotReroll(ulong itemGuid, uint index, RuneType runeType)
        {
            Item item = GetItem(itemGuid);
            if (item == null)
                throw new ArgumentException();

            if (item.RerollRuneSlot(index, runeType))
            {
                // TODO: Calculate Cost

                SendItemModify(item);
            }
            else
                player.Session.EnqueueMessageEncrypted(new ServerItemError
                {
                    ItemGuid = itemGuid,
                    ErrorCode = GenericError.CraftBadParams
                });
        }

        /// <summary>
        /// Used to Insert a list of Rune Item IDs into the supplied Guid matching an <see cref="Item"/>.
        /// </summary>
        public void RuneInsert(ulong itemGuid, uint[] runeItemIds)
        {
            Item item = GetItem(itemGuid);
            if (item == null)
                throw new ArgumentException();

            for (uint i = 0; i < runeItemIds.Length; i++)
            {
                // Nothing being set as part of this insert request, skip to next.
                if (runeItemIds[i] == 0)
                    continue;

                // Rune Socket missing, error.
                if (item.Runes[i] == null)
                    throw new ArgumentNullException();

                // Rune Socket is filled with this Rune already, skip to next.
                // We skip because on a Rune Insert packet, it sends a request with all runes it is expecting, even though only 1 can be added each time.
                if (item.Runes[i].RuneItem == runeItemIds[i])
                    continue;

                // Rune Socket must be empty to place a rune in it, error.
                if (item.Runes[i].RuneItem != null)
                    throw new InvalidOperationException();

                // Item missing to insert, error.
                if (!HasItem(runeItemIds[i]))
                    throw new InvalidOperationException();

                // TODO: Confirm any Socketing Requirements are met

                item.Runes[i].RuneItem = runeItemIds[i];
                ItemDelete(runeItemIds[i], 1u);
            }

            SendItemModifyGlyphs(item);

            // TODO: Update Stats/Properties
        }

        /// <summary>
        /// Used to remove a Rune from a given socket index for a supplied Guid matching an <see cref="Item"/>.
        /// </summary>
        public void RuneRemove(ulong itemGuid, uint socketIndex, bool recover, bool useServiceTokens)
        {
            Item item = GetItem(itemGuid);
            if (item == null)
                throw new ArgumentException();

            if (item.Runes[socketIndex] == null)
                throw new ArgumentException();

            if (!item.Runes[socketIndex].RuneItem.HasValue)
                throw new ArgumentException();

            if (recover)
            {
                if (GetInventorySlotsRemaining(InventoryLocation.Inventory) < 1u)
                {
                    player.SendGenericError(GenericError.ItemInventoryFull);
                    return;
                }

                ItemCreate(InventoryLocation.Inventory, item.Runes[socketIndex].RuneItem.Value, 1u, ItemUpdateReason.TradeskillGlyph);

                // TODO: Calculate Cost
            }

            item.Runes[socketIndex].RuneItem = null;

            SendItemModifyGlyphs(item);

            // TODO: Update Stats/Properties
        }

        private void SendItemModify(Item item)
        {
            player.Session.EnqueueMessageEncrypted(new ServerItemModify
            {
                ItemGuid          = item.Guid,
                ThresholdData     = 0u,
                RandomCircuitData = item.RandomCircuitData,
                RandomGlyphData   = item.RandomGlyphData
            });
        }

        private void SendItemModifyGlyphs(Item item)
        {
            player.Session.EnqueueMessageEncrypted(new ServerItemModifyGlyphs
            {
                ItemGuid        = item.Guid,
                RandomGlyphData = item.RandomGlyphData,
                Glyphs          = item.Runes.Values.Select(i => i.RuneItem ?? 0).ToList()
            });
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
