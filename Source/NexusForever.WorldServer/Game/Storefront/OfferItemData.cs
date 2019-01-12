using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class OfferItemData
    {
        public uint OfferId { get; private set; }
        public ushort ItemId { get; private set; }
        public uint Type { get; private set; }
        public uint Amount { get; private set; }

        public AccountItemEntry Entry { get; private set; }

        public OfferItemData(StoreOfferItemData model)
        {
            OfferId = model.Id;
            ItemId = model.ItemId;
            Type = model.Type;
            Amount = model.Amount;

            Entry = GameTableManager.AccountItem.GetEntry(ItemId);
            if (Entry == null)
                throw new ArgumentOutOfRangeException("ItemId");
        }

        public ServerStoreOffers.OfferGroup.Offer.OfferItemData BuildNetworkPacket()
        {
            return new ServerStoreOffers.OfferGroup.Offer.OfferItemData
            {
                Type = Type,
                AccountItemId = ItemId,
                Amount = Amount
            };
        }
    }
}
