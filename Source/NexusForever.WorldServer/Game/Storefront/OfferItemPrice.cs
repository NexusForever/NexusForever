using System;
using NexusForever.Database.World.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Storefront.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class OfferItemPrice : IBuildable<ServerStoreOffers.OfferGroup.Offer.OfferCurrencyData>
    {
        public uint OfferId { get; }
        public byte CurrencyId { get; }
        public float Price { get; }
        public DiscountType DiscountType { get; }
        public float DiscountValue { get; }
        public long DiscountTimeRemaining { get; }
        public long Expiry { get; }

        /// <summary>
        /// Create a new <see cref="OfferItemPrice"/> from an existing database model.
        /// </summary>
        public OfferItemPrice(StoreOfferItemPriceModel model)
        {
            OfferId               = model.Id;
            CurrencyId            = model.CurrencyId;
            Price                 = model.Price;
            DiscountType          = (DiscountType)model.DiscountType;
            DiscountValue         = model.DiscountValue;
            DiscountTimeRemaining = model.Field14;
            Expiry                = model.Expiry;
        }

        public float GetCurrencyValue()
        {
            if (DiscountValue == 0f)
                return (float)Math.Ceiling(Price);

            return (float)Math.Ceiling(Price / ((100f - DiscountValue) / 100)); // This gives the full price of the item
        }

        public ServerStoreOffers.OfferGroup.Offer.OfferCurrencyData Build()
        {
            return new ServerStoreOffers.OfferGroup.Offer.OfferCurrencyData
            {
                CurrencyId            = CurrencyId,
                Price                 = Price,
                DiscountType          = DiscountType,
                DiscountValue         = DiscountValue,
                DiscountTimeRemaining = (DiscountType != DiscountType.None ? 1 : -1), // Values more than 0 "enable" discount
                TimeSinceExpiry       = -1995405795 // Expiry
            };
        }
    }
}
