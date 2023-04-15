using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IItemInfo
    {
        uint Id { get; }
        Item2Entry Entry { get; }
        Item2FamilyEntry FamilyEntry { get; }
        Item2CategoryEntry CategoryEntry { get; }
        Item2TypeEntry TypeEntry { get; }
        ItemSlotEntry SlotEntry { get; }

        /// <summary>
        /// Returns if item can be equipped into an item slot.
        /// </summary>
        bool IsEquippable();

        /// <summary>
        /// Returns if item can be equipped into item slot <see cref="EquippedItem"/>.
        /// </summary>
        bool IsEquippableIntoSlot(EquippedItem bagIndex);

        /// <summary>
        /// Returns if item can be stacked with other items of the same type.
        /// </summary>
        bool IsStackable();

        /// <summary>
        /// Returns if item can be used as a bag for expanding inventory slots.
        /// </summary>
        bool IsEquippableBag();
    }
}