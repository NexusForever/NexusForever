using System.Collections;
using System.Collections.Generic;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class BuybackInfo : IEnumerable<BuybackItem>
    {
        private uint currentUniqueId;
        private readonly Dictionary<uint /*UniqueId*/, BuybackItem> buybackItems = new Dictionary<uint, BuybackItem>();

        /// <summary>
        /// Return stored <see cref="BuybackItem"/> with supplied unique id.
        /// </summary>
        public BuybackItem GetItem(uint uniqueId)
        {
            return buybackItems.TryGetValue(uniqueId, out BuybackItem buybackItem) ? buybackItem : null;
        }

        /// <summary>
        /// Create a new <see cref="BuybackItem"/> from sold <see cref="Item"/>.
        /// </summary>
        public uint AddItem(Item item, uint quantity, List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> currencyChange)
        {
            uint uniqueId = currentUniqueId++;
            buybackItems.Add(uniqueId, new BuybackItem(uniqueId, item, quantity, currencyChange));
            return uniqueId;
        }

        /// <summary>
        /// Remove stored <see cref="BuybackItem"/> with supplied unique id.
        /// </summary>
        public void RemoveItem(uint uniqueId)
        {
            buybackItems.Remove(uniqueId);
        }

        public IEnumerator<BuybackItem> GetEnumerator()
        {
            return buybackItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
