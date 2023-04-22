using System.Collections;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;

namespace NexusForever.Game.Entity
{
    public class BuybackInfo : IBuybackInfo
    {
        private uint currentUniqueId;
        private readonly Dictionary<uint /*UniqueId*/, IBuybackItem> buybackItems = new();

        /// <summary>
        /// Return stored <see cref="IBuybackItem"/> with supplied unique id.
        /// </summary>
        public IBuybackItem GetItem(uint uniqueId)
        {
            return buybackItems.TryGetValue(uniqueId, out IBuybackItem buybackItem) ? buybackItem : null;
        }

        /// <summary>
        /// Create a new <see cref="IBuybackItem"/> from sold <see cref="IItem"/>.
        /// </summary>
        public uint AddItem(IItem item, uint quantity, List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> currencyChange)
        {
            uint uniqueId = currentUniqueId++;
            buybackItems.Add(uniqueId, new BuybackItem(uniqueId, item, quantity, currencyChange));
            return uniqueId;
        }

        /// <summary>
        /// Remove stored <see cref="IBuybackItem"/> with supplied unique id.
        /// </summary>
        public void RemoveItem(uint uniqueId)
        {
            buybackItems.Remove(uniqueId);
        }

        public IEnumerator<IBuybackItem> GetEnumerator()
        {
            return buybackItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
