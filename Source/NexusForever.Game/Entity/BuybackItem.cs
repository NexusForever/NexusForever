using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Entity;
using NetworkBuybackItem = NexusForever.Network.World.Message.Model.Item.BuybackItem;

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
                Item2Id   = Item.Info.Id,
                Quantity = Quantity,
            };

            networkBuybackItem.CurrencyTypeId_First = CurrencyChange[0].CurrencyTypeId;
            networkBuybackItem.CurrencyAmount_First = CurrencyChange[0].CurrencyAmount;

            networkBuybackItem.CurrencyTypeId_Second = CurrencyChange[1].CurrencyTypeId;
            networkBuybackItem.CurrencyAmount_Second = CurrencyChange[1].CurrencyAmount;

            return networkBuybackItem;
        }
    }
}
