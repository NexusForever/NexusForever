using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Abstract
{
    public interface IItemManager
    {
        /// <summary>
        /// Id to be assigned to the next created item.
        /// </summary>
        ulong NextItemId { get; }

        void Initialise();

        /// <summary>
        /// Return <see cref="IItemInfo"/> with supplied id.
        /// </summary>
        IItemInfo GetItemInfo(uint id);

        /// <summary>
        /// Returns a collection of bag indexes for <see cref="InventoryLocation.Equipped"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        IEnumerable<EquippedItem> GetEquippedBagIndexes(ItemSlot slot);
    }
}