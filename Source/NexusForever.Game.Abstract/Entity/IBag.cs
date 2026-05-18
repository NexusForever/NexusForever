using NexusForever.Database.Character;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IBag : IDatabaseCharacter, IEnumerable<IItem>
    {
        InventoryLocation Location { get; }
        uint Slots { get; }
        uint SlotsRemaining { get; }

        /// <summary>
        /// Returns <see cref="IItem"/> with the supplied guid.
        /// </summary>
        IItem GetItem(ulong guid);

        /// <summary>
        /// Returns <see cref="IItem"/> found at the supplied bag index.
        /// </summary>
        IItem GetItem(uint bagIndex);

        /// <summary>
        /// Returns the first empty bag index.
        /// </summary>
        uint? GetFirstAvailableBagIndex();

        /// <summary>
        /// Return the first empty bag index for <see cref="ItemSlot"/>.
        /// </summary>
        uint? GetFirstAvailableBagIndex(ItemSlot slot);

        /// <summary>
        /// Add <see cref="Item"/> to the bag at the supplied bag index.
        /// </summary>
        void AddItem(IItem item, uint bagIndex);

        /// <summary>
        /// Remove existing <see cref="IItem"/> from the bag at the supplied bag index.
        /// </summary>
        void RemoveItem(IItem item);

        /// <summary>
        /// Move existing <see cref="IItem"/> in <see cref="IBag"/> to supplied empty bag index.
        /// </summary>
        void MoveItem(IItem item, uint bagIndex);

        /// <summary>
        /// Swap 2 existing <see cref="IItem"/> bag indexes in <see cref="IBag"/> with each other.
        /// </summary>
        void SwapItem(IItem item, IItem item2);

        /// <summary>
        /// Resize bag with supplied capacity change.
        /// </summary>
        void Resize(int capacityChange);
    }
}