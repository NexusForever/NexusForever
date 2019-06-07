using System.Collections.Generic;
using NexusForever.Shared;
using NexusForever.WorldServer.Game.Entity.Static;

namespace NexusForever.WorldServer.Game.Entity
{
    public class BuybackItem : IUpdate
    {
        public uint UniqueId { get; }
        public Item Item { get; }
        public uint Quantity { get; }
        public List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> CurrencyChange { get; }

        public bool HasExpired => timeToExpire <= 0d;
        private double timeToExpire = 1800d;

        public BuybackItem(uint uniqueId, Item item, uint quantity, List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> currencyChange)
        {
            UniqueId       = uniqueId;
            Item           = item;
            Quantity       = quantity;
            CurrencyChange = currencyChange;
        }

        public void Update(double lastTick)
        {
            timeToExpire -= lastTick;
        }
    }
}
