using System;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class OfferItemData
    {
        public uint OfferId { get; }
        public ushort ItemId { get; }
        public uint Type { get; }
        public uint Amount { get; }

        public AccountItemEntry Entry { get; }

        /// <summary>
        /// Create a new <see cref="OfferItemData"/>
        /// </summary>
        public OfferItemData(StoreOfferItemData model)
        {
            OfferId = model.Id;
            ItemId  = model.ItemId;
            Type    = model.Type;
            Amount  = model.Amount;

            Entry = GameTableManager.Instance.AccountItem.GetEntry(ItemId);
            if (Entry == null)
                throw new ArgumentException("ItemId");
        }

        public ServerStoreOffers.OfferGroup.Offer.OfferItemData BuildNetworkPacket()
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
