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

        private Item[] items;
        public uint SlotsRemaining { get; private set; }

        public Bag(InventoryLocation location, uint capacity)
        {
            Location = location;
            items    = new Item[capacity];

            SlotsRemaining = capacity;

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
        /// Returns the first empty bag index, if the bag is full <see cref="uint.MaxValue"/> is returned.
        /// </summary>
        public uint GetFirstAvailableBagIndex()
        {
            for (int i = 0; i < items.Length; i++)
                if (items[i] == null)
                    return (uint)i;

            return uint.MaxValue;
        }

        /// <summary>
        /// Returns the first empty bag index, if the bag is full <see cref="uint.MaxValue"/> is returned.
        /// </summary>
        public uint GetFirstAvailableBagIndexAfterIndex(int index)
        {
            for (int i = index + 1; i < items.Length; i++)
                if (items[i] == null)
                    return (uint)i;

            return uint.MaxValue;
        }

        /// <summary>
        /// Return the amount of empty bag indexes.
        /// </summary>
        public uint GetFreeBagIndexCount()
        {
            return (uint)items.Count(i => i == null);
        }

        /// <summary>
        /// Add <see cref="Item"/> to the bag at the supplied bag index.
        /// </summary>
        public void AddItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Location != Location)
                throw new ArgumentException();
            if (items[item.BagIndex] != null)
                throw new ArgumentException();

            items[item.BagIndex] = item;

            log.Trace($"Added item 0x{item.Guid:X16} to bag {Location} at index {item.BagIndex}.");

            SlotsRemaining = Math.Clamp(SlotsRemaining - 1, 0, (uint)items.Length);
        }

        /// <summary>
        /// Remove <see cref="Item"/> from the bag at the supplied bag index.
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

            item.Location = InventoryLocation.None;
            item.BagIndex = 0u;

            SlotsRemaining = Math.Clamp(SlotsRemaining + 1, 0, (uint)items.Length);
        }

        /// <summary>
        /// Returns bag's current size
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return items.Length;
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
