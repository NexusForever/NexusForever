using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NexusForever.Database.Character;
using NexusForever.WorldServer.Game.Entity.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Bag : IEnumerable<Item>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public InventoryLocation Location { get; }
        public uint Slots => (uint)items.Length;
        public uint SlotsRemaining { get; private set; }

        private Item[] items;

        public Bag(InventoryLocation location, uint capacity)
        {
            Location       = location;
            SlotsRemaining = capacity;
            items          = new Item[capacity];

            log.Trace($"Initialised new bag {Location} with {capacity} slots.");
        }

        public void Save(CharacterContext context)
        {
            for (uint i = 0; i < items.Length; ++i)
            {
                // Item does not exist in slot
                if (items[i] == null)
                    continue;

                // Item has incorrect slot, setting the BagIndex to the current index
                if (i != items[i].BagIndex)
                {
                    log.Warn($"Item with guid: 0x{items[i].Guid:X16} has incorrect slot: {items[i].BagIndex}, setting to slot: {i}");
                    items[i].BagIndex = i;
                }

                items[i].Save(context);
            }
        }

        /// <summary>
        /// Returns <see cref="Item"/> with the supplied guid.
        /// </summary>
        public Item GetItem(ulong guid)
        {
            return items.SingleOrDefault(i => i.Guid == guid);
        }

        /// <summary>
        /// Returns <see cref="Item"/> found at the supplied bag index.
        /// </summary>
        public Item GetItem(uint bagIndex)
        {
            if (bagIndex >= items.Length)
                throw new ArgumentOutOfRangeException();

            return items[(int)bagIndex];
        }

        /// <summary>
        /// Returns the first empty bag index.
        /// </summary>
        public uint? GetFirstAvailableBagIndex()
        {
            for (uint bagIndex = 0u; bagIndex < items.Length; bagIndex++)
                if (items[bagIndex] == null)
                    return bagIndex;

            return null;
        }

        /// <summary>
        /// Return the first empty bag index for <see cref="ItemSlot"/>.
        /// </summary>
        public uint? GetFirstAvailableBagIndex(ItemSlot slot)
        {
            if (Location != InventoryLocation.Equipped)
                return null;

            // find first free bag index, some items can be equipped into multiple slots
            foreach (uint bagIndex in ItemManager.Instance.GetEquippedBagIndexes(slot))
                if (items[bagIndex] == null)
                    return bagIndex;

            return null;
        }

        /// <summary>
        /// Add <see cref="Item"/> to the bag at the supplied bag index.
        /// </summary>
        public void AddItem(Item item, uint bagIndex)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Location != InventoryLocation.None)
                throw new ArgumentException();
            if (items[bagIndex] != null)
                throw new ArgumentException();

            item.Location = Location;
            item.BagIndex = bagIndex;
            items[item.BagIndex] = item;

            checked
            {
                SlotsRemaining--;
            }

            log.Trace($"Added item 0x{item.Guid:X16} to bag {Location} at index {item.BagIndex}.");
        }

        /// <summary>
        /// Remove existing <see cref="Item"/> from the bag at the supplied bag index.
        /// </summary>
        public void RemoveItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Location != Location)
                throw new ArgumentException();
            if (items[item.BagIndex] == null)
                throw new ArgumentException();
            
            items[item.BagIndex] = null;

            log.Trace($"Removed item 0x{item.Guid:X16} from bag {Location} at index {item.BagIndex}.");

            item.PreviousLocation = item.Location;
            item.PreviousBagIndex = item.BagIndex;
            item.Location         = InventoryLocation.None;
            item.BagIndex         = 0u;

            checked
            {
                SlotsRemaining++;
            }

            if (SlotsRemaining > Slots)
                throw new InvalidOperationException();
        }

        /// <summary>
        /// Move existing <see cref="Item"/> in <see cref="Bag"/> to supplied empty bag index.
        /// </summary>
        public void MoveItem(Item item, uint bagIndex)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Location != Location)
                throw new ArgumentException();
            if (items[item.BagIndex] == null)
                throw new ArgumentException();
            if (items[bagIndex] != null)
                throw new ArgumentException();

            item.PreviousLocation = item.Location;
            item.PreviousBagIndex = item.BagIndex;

            items[item.BagIndex] = null;

            item.BagIndex = bagIndex;
            items[bagIndex] = item;

            log.Trace($"Moved item 0x{item.Guid:X16} from bag {Location} at index {item.PreviousBagIndex} to index {item.BagIndex}.");
        }

        /// <summary>
        /// Swap 2 existing <see cref="Item"/> bag indexes in <see cref="Bag"/> with each other.
        /// </summary>
        public void SwapItem(Item item, Item item2)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item2 == null)
                throw new ArgumentNullException(nameof(item2));
            if (item.Location != Location)
                throw new ArgumentException();
            if (item2.Location != Location)
                throw new ArgumentException();
            if (items[item.BagIndex] == null)
                throw new ArgumentException();
            if (items[item2.BagIndex] == null)
                throw new ArgumentException();

            item.PreviousLocation  = item.Location;
            item2.PreviousLocation = item2.Location;
            item.PreviousBagIndex  = item.BagIndex;
            item2.PreviousBagIndex = item2.BagIndex;

            items[item.BagIndex] = item2;
            item2.BagIndex = item.BagIndex;

            items[item2.PreviousBagIndex] = item;
            item.BagIndex = item2.PreviousBagIndex;

            log.Trace($"Swapped items 0x{item.Guid:X16} and 0x{item2.Guid:X16} from bag {Location} between index {item.BagIndex} and {item2.BagIndex}.");
        }

        /// <summary>
        /// Resize bag with supplied capacity change.
        /// </summary>
        public void Resize(int capacityChange)
        {
            if (items.Length + capacityChange < 0)
                throw new ArgumentOutOfRangeException(nameof(capacityChange));

            Array.Resize(ref items, items.Length + capacityChange);
            SlotsRemaining = (uint)(SlotsRemaining + capacityChange);

            log.Trace($"Resized bag {Location} from {items.Length - capacityChange} to {items.Length} slots.");
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return items
                .Where(i => i != null)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
