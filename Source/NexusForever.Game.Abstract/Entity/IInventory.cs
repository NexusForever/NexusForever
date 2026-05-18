using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model.Shared;
using NexusForever.Network.World.Message.Static;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IInventory : IDatabaseCharacter, IUpdate, IEnumerable<IBag>
    {
        /// <summary>
        /// Returns if <see cref="InventoryLocation"/> and bag index is a visible item slot.
        /// </summary>
        bool IsVisualItemSlot(InventoryLocation location, uint bagIndex);

        /// <summary>
        /// Returns if <see cref="InventoryLocation"/> and bag index is a bag slot.
        /// </summary>
        bool IsEquippableBagSlot(InventoryLocation location, uint bagIndex);

        /// <summary>
        /// Returns if <see cref="InventoryLocation"/> and bag index is a bank bag slot.
        /// </summary>
        bool IsEquippableBankBagSlot(InventoryLocation location, uint bagIndex);

        /// <summary>
        /// Return if <see cref="InventoryLocation"/> has no free bag indexes remaining.
        /// </summary>
        bool IsInventoryFull(InventoryLocation location);

        /// <summary>
        /// Return the amount of free bag indexes in <see cref="InventoryLocation.Inventory"/>.
        /// </summary>
        uint GetInventorySlotsRemaining(InventoryLocation location);

        /// <summary>
        /// Returns if the count of items with id exists in <see cref="InventoryLocation.Inventory"/>.
        /// </summary>
        bool HasItemCount(uint itemId, uint count);

        /// <summary>
        /// Return <see cref="IItem"/> at supplied <see cref="ItemLocation"/>.
        /// </summary>
        IItem GetItem(ItemLocation itemLocation);

        /// <summary>
        /// Return <see cref="IItem"/> at supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        IItem GetItem(InventoryLocation location, uint bagIndex);

        /// <summary>
        /// Return <see cref="IItem"/> with supplied guid.
        /// </summary>
        IItem GetItem(ulong guid);

        /// <summary>
        /// Returns <see cref="IItemVisual"/> for any visible items.
        /// </summary>
        IEnumerable<IItemVisual> GetItemVisuals();

        /// <summary>
        /// Create a new <see cref="IItem"/> from supplied <see cref="Spell4BaseEntry"/> in the first available <see cref="InventoryLocation.Ability"/> bag slot.
        /// </summary>
        IItem SpellCreate(Spell4BaseEntry spell4BaseEntry, ItemUpdateReason reason = ItemUpdateReason.NoReason);

        /// <summary>
        /// Create a new <see cref="IItem"/> in the first available inventory bag index or stack.
        /// </summary>
        void ItemCreate(InventoryLocation location, uint itemId, uint count, ItemUpdateReason reason = ItemUpdateReason.NoReason, uint charges = 0);

        /// <summary>
        /// Create a new <see cref="IItem"/> in the first available inventory bag index or stack.
        /// </summary>
        void ItemCreate(InventoryLocation location, IItemInfo info, uint count, ItemUpdateReason reason = ItemUpdateReason.NoReason, uint charges = 0);

        /// <summary>
        /// Returns if <see cref="IItem"/> can be moved to supplied <see cref="ItemLocation"/>.
        /// </summary>
        GenericError? CanMoveItem(IItem item, ItemLocation location);

        /// <summary>
        /// Returns if <see cref="IItem"/> can be moved to supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        GenericError? CanMoveItem(IItem item, InventoryLocation location, uint bagIndex);

        /// <summary>
        /// Move <see cref="IItem"/> to supplied <see cref="ItemLocation"/>.
        /// </summary>
        void ItemMove(IItem item, ItemLocation location);

        // <summary>
        /// Move <see cref="IItem"/> to supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        void ItemMove(IItem item, InventoryLocation location, uint bagIndex);

        /// <summary>
        /// Split a subset of <see cref="IItem"/> to create a new <see cref="IItem"/> of split amount
        /// </summary>
        void ItemSplit(ulong itemGuid, ItemLocation newItemLocation, uint count);

        /// <summary>
        /// Delete <see cref="IItem"/> at supplied <see cref="ItemLocation"/>, this is called directly from a packet hander.
        /// </summary>
        IItem ItemDelete(ItemLocation from, ItemUpdateReason reason = ItemUpdateReason.Loot);

        /// <summary>
        /// Delete a supplied amount of an item.
        /// </summary>
        void ItemDelete(uint itemId, uint count = 1, ItemUpdateReason reason = ItemUpdateReason.Loot);

        /// <summary>
        /// Remove <see cref="IItem"/> from this player's inventory without deleting the item from the DB
        /// </summary>
        void ItemRemove(IItem item, ItemUpdateReason reason = ItemUpdateReason.NoReason);

        // <summary>
        /// Add <see cref="IItem"/> in the first available bag index for the given <see cref="InventoryLocation"/> .
        /// </summary>
        void AddItem(IItem item, InventoryLocation location, ItemUpdateReason reason = ItemUpdateReason.NoReason);

        /// <summary>
        /// Apply stack updates and deletion to <see cref="IItem"/> on use
        /// </summary>
        bool ItemUse(IItem item);

        void ItemMoveToSupplySatchel(IItem item, uint amount);
    }
}