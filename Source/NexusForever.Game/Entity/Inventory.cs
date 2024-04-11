using System.Collections;
using System.Diagnostics;
using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Achievement;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;
using NLog;

namespace NexusForever.Game.Entity
{
    public class Inventory : IInventory
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static ulong ItemLocationToDragDropData(InventoryLocation location, ushort slot)
        {
            // TODO: research this more, client version of this is more complex
            return (ulong)location << 8 | slot;
        }

        private readonly ulong characterId;
        private readonly IPlayer player;
        private readonly Dictionary<InventoryLocation, IBag> bags = new();
        private readonly List<IItem> deletedItems = new();

        /// <summary>
        /// Create a new <see cref="IInventory"/> from <see cref="IPlayer"/> database model.
        /// </summary>
        public Inventory(IPlayer owner, CharacterModel model)
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
        /// Create a new <see cref="IInventory"/> from supplied <see cref="CharacterCreationEntry"/>.
        /// </summary>
        public Inventory(ulong owner, CharacterCreationEntry creationEntry)
        {
            characterId = owner;

            foreach ((InventoryLocation location, uint defaultCapacity) in AssetManager.InventoryLocationCapacities)
                bags.Add(location, new Bag(location, defaultCapacity));

            foreach (uint itemId in creationEntry.ItemIds.Where(i => i != 0u))
            {
                IItemInfo info = ItemManager.Instance.GetItemInfo(itemId);
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
            foreach (IItem item in deletedItems)
                item.Save(context);

            deletedItems.Clear();

            foreach (IBag bag in bags.Values.Where(b => b.Location != InventoryLocation.Ability))
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
                   && (EquippedItem)bagIndex is EquippedItem.Bag0 or EquippedItem.Bag1 or EquippedItem.Bag2 or EquippedItem.Bag3;
        }

        /// <summary>
        /// Returns if <see cref="InventoryLocation"/> and bag index is a bank bag slot.
        /// </summary>
        public bool IsEquippableBankBagSlot(InventoryLocation location, uint bagIndex)
        {
            return location == InventoryLocation.Equipped
                && (EquippedItem)bagIndex is EquippedItem.BankBag0 or EquippedItem.BankBag1 or EquippedItem.BankBag2
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
            IBag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            return bag.SlotsRemaining;
        }

        /// <summary>
        /// Returns if the count of items with id exists in <see cref="InventoryLocation.Inventory"/>.
        /// </summary>
        public bool HasItemCount(uint itemId, uint count)
        {
            IBag bag = GetBag(InventoryLocation.Inventory);
            if (bag == null)
                throw new ArgumentException();

            foreach (IItem item in bag.Where(i => i.Id == itemId))
            {
                count -= Math.Min(count, item.StackCount);
                if (count == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Return <see cref="IItem"/> at supplied <see cref="ItemLocation"/>.
        /// </summary>
        public IItem GetItem(ItemLocation itemLocation)
        {
            return GetItem(itemLocation.Location, itemLocation.BagIndex);
        }

        /// <summary>
        /// Return <see cref="IItem"/> at supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        public IItem GetItem(InventoryLocation location, uint bagIndex)
        {
            IBag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            IItem item = bag.GetItem(bagIndex);
            if (item == null)
                throw new ArgumentException();

            return item;
        }

        /// <summary>
        /// Return <see cref="IItem"/> with supplied guid.
        /// </summary>
        public IItem GetItem(ulong guid)
        {
            return bags.Values
                .SelectMany(b => b)
                .FirstOrDefault(i => i.Guid == guid);
        }

        /// <summary>
        /// Returns <see cref="IItemVisual"/> for any visible items.
        /// </summary>
        public IEnumerable<IItemVisual> GetItemVisuals()
        {
            IBag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);

            foreach (IItem item in bag)
            {
                if (!IsVisualItemSlot(item.Location, item.BagIndex))
                    continue;

                yield return new ItemVisual
                {
                    Slot      = (ItemSlot)item.Info.TypeEntry.ItemSlotId,
                    DisplayId = item.Info.GetDisplayId()
                };
            }
        }

        /// <summary>
        /// Create a new <see cref="IItem"/> from supplied <see cref="Spell4BaseEntry"/> in the first available <see cref="InventoryLocation.Ability"/> bag slot.
        /// </summary>
        public IItem SpellCreate(Spell4BaseEntry spell4BaseEntry, ItemUpdateReason reason = ItemUpdateReason.NoReason)
        {
            if (spell4BaseEntry == null)
                throw new ArgumentNullException();

            var spell = new Item(characterId, spell4BaseEntry);
            AddItem(spell, InventoryLocation.Ability, reason);

            return spell;
        }

        /// <summary>
        /// Create a new <see cref="IItem"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(InventoryLocation location, uint itemId, uint count, ItemUpdateReason reason = ItemUpdateReason.NoReason, uint charges = 0)
        {
            IItemInfo info = ItemManager.Instance.GetItemInfo(itemId);
            if (info == null)
                throw new ArgumentNullException();

            ItemCreate(location, info, count, reason, charges);
        }

        /// <summary>
        /// Create a new <see cref="IItem"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(InventoryLocation location, IItemInfo info, uint count, ItemUpdateReason reason = ItemUpdateReason.NoReason, uint charges = 0)
        {
            if (info == null)
                throw new ArgumentNullException();

            IBag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            // update any existing stacks before creating new items
            if (info.IsStackable())
            {
                foreach (IItem item in bag.Where(i => i.Info.Id == info.Id))
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
                            Item   = item.Build(),
                            Reason = reason
                        }
                    });
                }

                count -= item.StackCount;
            }
        }

        /// <summary>
        /// Returns if <see cref="IItem"/> can be moved to supplied <see cref="ItemLocation"/>.
        /// </summary>
        public GenericError? CanMoveItem(IItem item, ItemLocation location)
        {
            return CanMoveItem(item, location.Location, location.BagIndex);
        }

        /// <summary>
        /// Returns if <see cref="IItem"/> can be moved to supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        public GenericError? CanMoveItem(IItem item, InventoryLocation location, uint bagIndex)
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

            IBag bag = GetBag(location);
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
        /// Move <see cref="IItem"/> to supplied <see cref="ItemLocation"/>.
        /// </summary>
        public void ItemMove(IItem item, ItemLocation location)
        {
            ItemMove(item, location.Location, location.BagIndex);
        }

        /// <summary>
        /// Move <see cref="IItem"/> to supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        public void ItemMove(IItem item, InventoryLocation location, uint bagIndex)
        {
            if (item == null)
                throw new ArgumentNullException();

            IBag bag = GetBag(item.Location);
            if (bag == null)
                throw new ArgumentException();

            IBag dstBag = GetBag(location);
            if (dstBag == null)
                throw new ArgumentException();

            IItem dstItem = dstBag.GetItem(bagIndex);

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
        /// Split a subset of <see cref="IItem"/> to create a new <see cref="IItem"/> of split amount
        /// </summary>
        public void ItemSplit(ulong itemGuid, ItemLocation newItemLocation, uint count)
        {
            IItem item = GetItem(itemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            if (item.Info.IsEquippableBag())
                throw new InvalidPacketValueException();

            if (item.Info.Entry.MaxStackCount <= 1u)
                throw new InvalidPacketValueException();

            if (count >= item.StackCount)
                throw new InvalidPacketValueException();

            IBag dstBag = GetBag(newItemLocation.Location);
            if (dstBag == null)
                throw new InvalidPacketValueException();

            IItem dstItem = dstBag.GetItem(newItemLocation.BagIndex);
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
                        Item = newItem.Build(),
                        Reason = ItemUpdateReason.NoReason
                    }
                });
            }

            ItemStackCountUpdate(item, item.StackCount - count);
        }

        /// <summary>
        /// Delete <see cref="IItem"/> at supplied <see cref="ItemLocation"/>, this is called directly from a packet hander.
        /// </summary>
        public IItem ItemDelete(ItemLocation from, ItemUpdateReason reason = ItemUpdateReason.Loot)
        {
            IBag srcBag = GetBag(from.Location);
            if (srcBag == null)
                throw new InvalidPacketValueException();

            IItem srcItem = srcBag.GetItem(from.BagIndex);
            if (srcItem == null)
                throw new InvalidPacketValueException();

            return ItemDelete(srcBag, srcItem, reason);
        }

        private IItem ItemDelete(IBag bag, IItem item, ItemUpdateReason reason)
        {
            bag.RemoveItem(item);
            if (!item.PendingCreate)
            {
                item.EnqueueDelete(true);
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
        /// Delete a supplied amount of an <see cref="IItem"/>.
        /// </summary>
        public void ItemDelete(uint itemId, uint count = 1u, ItemUpdateReason reason = ItemUpdateReason.Loot)
        {
            IBag bag = GetBag(InventoryLocation.Inventory);
            foreach (IItem item in bag.Where(i => i.Id == itemId))
            {
                if (item.StackCount > count)
                {
                    ItemStackCountUpdate(item, item.StackCount - count);
                    count = 0;
                }
                else
                {
                    ItemDelete(bag, item, reason);
                    count -= item.StackCount;
                }

                if (count == 0)
                    break;
            }
        }

        /// <summary>
        /// Remove <see cref="IItem"/> from this player's inventory without deleting the item from the DB
        /// </summary>
        public void ItemRemove(IItem item, ItemUpdateReason reason = ItemUpdateReason.NoReason)
        {
            if (item == null)
                throw new ArgumentNullException();

            IBag srcBag = GetBag(item.Location);
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
        /// Add <see cref="IItem"/> in the first available bag index for the given <see cref="InventoryLocation"/> .
        /// </summary>
        public void AddItem(IItem item, InventoryLocation location, ItemUpdateReason reason = ItemUpdateReason.NoReason)
        {
            IBag bag = GetBag(location);
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
                        Item   = item.Build(),
                        Reason = reason
                    }
                });
            }
        }

        /// <summary>
        /// Add <see cref="IItem"/> to the supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        private void AddItem(IItem item, InventoryLocation location, uint bagIndex)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location != InventoryLocation.None)
                throw new ArgumentException();

            IBag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            bag.AddItem(item, bagIndex);

            if (location == InventoryLocation.Equipped && player != null)
                ApplyProperties(item);

            if (IsVisualItemSlot(item.Location, item.BagIndex) && player != null)
                player.AddVisual((ItemSlot)item.Info.TypeEntry.ItemSlotId, item.Info.GetDisplayId());

            if (IsEquippableBagSlot(item.Location, item.BagIndex))
                InventoryResize(InventoryLocation.Inventory, (int)item.Info.Entry.MaxStackCount);
            if (IsEquippableBankBagSlot(item.Location, item.BagIndex))
                InventoryResize(InventoryLocation.PlayerBank, (int)item.Info.Entry.MaxStackCount);
        }

        /// <summary>
        /// Remove <see cref="IItem"/> from it's current <see cref="InventoryLocation"/> and bag slot.
        /// </summary>
        private void RemoveItem(IItem item)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location == InventoryLocation.None)
                throw new ArgumentException();

            IBag bag = GetBag(item.Location);
            if (bag == null)
                throw new ArgumentException();

            if (item.Location == InventoryLocation.Equipped && player != null)
                RemoveProperties(item);

            if (IsEquippableBagSlot(item.Location, item.BagIndex) && item.Info.IsEquippableBag())
                InventoryResize(InventoryLocation.Inventory, (int)-item.Info.Entry.MaxStackCount);
            if (IsEquippableBankBagSlot(item.Location, item.BagIndex) && item.Info.IsEquippableBag())
                InventoryResize(InventoryLocation.PlayerBank, (int)-item.Info.Entry.MaxStackCount);

            bag.RemoveItem(item);

            if (IsVisualItemSlot(item.PreviousLocation, item.PreviousBagIndex) && player != null)
                player.RemoveVisual((ItemSlot)item.Info.TypeEntry.ItemSlotId);
        }

        /// <summary>
        /// Resize <see cref="InventoryLocation"/> with supplied capacity change.
        /// </summary>
        private void InventoryResize(InventoryLocation location, int capacityChange)
        {
            IBag bag = GetBag(location);
            if (bag == null)
                throw new ArgumentException();

            if (capacityChange < 0)
            {
                for (uint bagIndex = bag.Slots - 1; bagIndex >= bag.Slots + capacityChange; bagIndex--)
                {
                    IItem item = bag.GetItem(bagIndex);
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
        /// Update <see cref="IItem"/> with supplied stack count.
        /// </summary>
        private void ItemStackCountUpdate(IItem item, uint stackCount, ItemUpdateReason reason = ItemUpdateReason.NoReason)
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
        /// Apply stack updates and deletion to <see cref="IItem"/> on use
        /// </summary>
        public bool ItemUse(IItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Item is null.");

            // This should only apply for re-usable items, like Quest Clickies.
            if (item.Info.Entry.MaxCharges == 0 && item.Info.Entry.MaxStackCount == 1)
                return true;

            if (item.Charges <= 0 && item.Info.Entry.MaxCharges > 1 || item.StackCount <= 0 && item.Info.Entry.MaxStackCount > 1)
                return false;

            player.AchievementManager.CheckAchievements(player, AchievementType.ItemConsume, item.Id);

            if (item.Charges >= 1 && item.Info.Entry.MaxStackCount == 1)
                item.Charges--;

            if (item.Info.Entry.MaxStackCount > 1 && item.StackCount > 0)
                ItemStackCountUpdate(item, item.StackCount - 1);

            // TODO: Set Deletion reason to 1, when consuming a single charge item.
            if (item.StackCount == 0 && item.Info.Entry.MaxStackCount > 1 || item.Charges == 0 && item.Info.Entry.MaxCharges > 0)
            {
                ItemDelete(new ItemLocation
                {
                    Location = item.Location,
                    BagIndex = item.BagIndex
                }, ItemUpdateReason.ConsumeCharge);
            }

            return true;
        }

        public void ItemMoveToSupplySatchel(IItem item, uint amount)
        {
            if (player.SupplySatchelManager.IsFull(item))
                return;

            uint amountRemaining = player.SupplySatchelManager.AddAmount(item, amount);
            if (amountRemaining > 0)
                ItemStackCountUpdate(item, item.StackCount - amount + amountRemaining);
            else if (amount < item.StackCount)
                ItemStackCountUpdate(item, item.StackCount - amount);
            else
                ItemDelete(new ItemLocation
                    {
                        Location = item.Location,
                        BagIndex = item.BagIndex
                    }, ItemUpdateReason.MaterialBagConversion);
        }

        private IBag GetBag(InventoryLocation location)
        {
            return bags.TryGetValue(location, out IBag container) ? container : null;
        }

        private void ApplyProperties(IItem item)
        {
            foreach (KeyValuePair<Property, float> property in item.Info.Properties)
                player.AddItemProperty(property.Key, (ItemSlot)item.Info.SlotEntry.Id, property.Value);

            // TODO: Add properties from item's runes
        }

        private void RemoveProperties(IItem item)
        {
            foreach (KeyValuePair<Property, float> property in item.Info.Properties)
                player.RemoveItemProperty(property.Key, (ItemSlot)item.Info.SlotEntry.Id);

            // TODO: Remove properties from item's runes
        }

        public IEnumerator<IBag> GetEnumerator()
        {
            return bags.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
