using System;
using NexusForever.Database.World.Model;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class OfferItemData : IBuildable<ServerStoreOffers.OfferGroup.Offer.OfferItemData>
    {
        public uint OfferId { get; }
        public ushort ItemId { get; }
        public uint Type { get; }
        public uint Amount { get; }

        public AccountItemEntry Entry { get; }

        /// <summary>
        /// Create a new <see cref="OfferItemData"/>
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
