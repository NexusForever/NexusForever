using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Storefront;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Storefront
{
    public class OfferItemData : IOfferItemData
    {
        public uint OfferId { get; }
        public ushort ItemId { get; }
        public uint Type { get; }
        public uint Amount { get; }
        public AccountItemEntry Entry { get; }

        /// <summary>
        /// Create a new <see cref="IOfferItemData"/>
        /// </summary>
        public OfferItemData(StoreOfferItemDataModel model)
        {
            OfferId = model.Id;
            ItemId  = model.ItemId;
            Type    = model.Type;
            Amount  = model.Amount;

            Entry = GameTableManager.Instance.AccountItem.GetEntry(ItemId);
            if (Entry == null)
                throw new ArgumentException("ItemId");
        }

        public ServerStoreOffers.OfferGroup.Offer.OfferItemData Build()
        {
            return new ServerStoreOffers.OfferGroup.Offer.OfferItemData
            {
                Type          = Type,
                AccountItemId = ItemId,
                Amount        = Amount
            };
        }
    }
}
