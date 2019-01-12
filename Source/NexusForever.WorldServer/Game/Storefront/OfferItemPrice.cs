using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Game.Storefront.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class OfferItemPrice
    {
        public uint OfferId { get; set; }
        public byte CurrencyId { get; set; }
        public float Price { get; set; }
        public DiscountType DiscountType { get; set; }
        public float DiscountValue { get; set; }
        public long DiscountTimeRemaining { get; set; }
        public long Expiry { get; set; }

        public OfferItemPrice(StoreOfferItemPrice model)
        {
            OfferId = model.Id;
            CurrencyId = model.CurrencyId;
            Price = model.Price;
            DiscountType = (DiscountType)model.DiscountType;
            DiscountValue = model.DiscountValue;
            DiscountTimeRemaining = model.Field14;
            Expiry = model.Expiry;
        }

        public float GetCurrencyValue()
        {
            if (CurrencyId == 11 && GlobalStorefrontManager.ForcedProtobucksPrice > 0)
                return GlobalStorefrontManager.ForcedProtobucksPrice;
            if (CurrencyId == 6 && GlobalStorefrontManager.ForcedOmnibitsPrice > 0)
                return GlobalStorefrontManager.ForcedOmnibitsPrice;

            if (DiscountValue == 0f)
                return (float)Math.Ceiling(Price);
            else
            {
                return (float)Math.Ceiling(Price / ((100f - DiscountValue) / 100)); // This gives the full price of the item
            }
        }

        public ServerStoreOffers.OfferGroup.Offer.OfferCurrencyData BuildNetworkPacket()
        {
            return new ServerStoreOffers.OfferGroup.Offer.OfferCurrencyData
            {
                CurrencyId = CurrencyId,
                Price = Price,
                DiscountType = (CurrencyId == 11 && GlobalStorefrontManager.ForcedProtobucksPrice > 0) || (CurrencyId == 6 && GlobalStorefrontManager.ForcedOmnibitsPrice > 0) ? 0 : DiscountType,
                DiscountValue = (CurrencyId == 11 && GlobalStorefrontManager.ForcedProtobucksPrice > 0) || (CurrencyId == 6 && GlobalStorefrontManager.ForcedOmnibitsPrice > 0) ? 0 : DiscountValue,
                DiscountTimeRemaining = (CurrencyId == 11 && GlobalStorefrontManager.ForcedProtobucksPrice > 0) || (CurrencyId == 6 && GlobalStorefrontManager.ForcedOmnibitsPrice > 0) ? -1 : (DiscountType > 0 ? 1 : -1), // Values more than 0 "enable" discount
                TimeSinceExpiry = -1995405795 // Expiry
            };
        }
    }
}
