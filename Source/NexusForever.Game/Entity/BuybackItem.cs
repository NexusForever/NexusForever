using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NetworkBuybackItem = NexusForever.Network.World.Message.Model.Shared.BuybackItem;

namespace NexusForever.Game.Entity
{
    public class BuybackItem : IBuybackItem
    {
        public uint UniqueId { get; }
        public IItem Item { get; }
        public uint Quantity { get; }
        public List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> CurrencyChange { get; }

        public bool HasExpired => timeToExpire <= 0d;
        private double timeToExpire = 1800d;

        public BuybackItem(uint uniqueId, IItem item, uint quantity, List<(CurrencyType CurrencyTypeId, ulong CurrencyAmount)> currencyChange)
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

        public NetworkBuybackItem Build()
        {
            var networkBuybackItem = new NetworkBuybackItem
            {
                UniqueId = UniqueId,
                ItemId   = Item.Info.Id,
                Quantity = Quantity,
            };

            for (int i = 0; i < networkBuybackItem.CurrencyTypeId.Length; i++)
            {
                if (i >= CurrencyChange.Count)
                    continue;

                networkBuybackItem.CurrencyTypeId[i] = CurrencyChange[i].CurrencyTypeId;
                networkBuybackItem.CurrencyAmount[i] = CurrencyChange[i].CurrencyAmount;
            }

            return networkBuybackItem;
        }
    }
}
