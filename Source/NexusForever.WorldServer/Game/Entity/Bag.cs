﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NexusForever.WorldServer.Game.Entity.Static;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Bag : IEnumerable<Item>
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        public InventoryLocation Location { get; }

        private Item[] items;

        public Bag(InventoryLocation location, uint capacity)
        {
            Location = location;
            items    = new Item[capacity];

            log.Trace($"Initialised new bag {Location} with {capacity} slots.");
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
        }

        /// <summary>
        /// Resize bag with supplied capacity change.
        /// </summary>
        public void Resize(int capacityChange)
        {
            if (items.Length + capacityChange < 0)
                throw new ArgumentOutOfRangeException(nameof(capacityChange));
            if (items.Length + capacityChange < items.Length)
                throw new ArgumentOutOfRangeException(nameof(capacityChange));

            Array.Resize(ref items, items.Length + capacityChange);

            log.Trace($"Resized bag {Location} from {items.Length} to {items.Length + capacityChange} slots.");
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
