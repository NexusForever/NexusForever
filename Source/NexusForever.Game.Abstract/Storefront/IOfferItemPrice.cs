using NexusForever.Game.Static.Storefront;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Storefront
{
    public interface IOfferItemPrice : INetworkBuildable<ServerStoreOffers.OfferGroup.Offer.OfferCurrencyData>
    {
        uint OfferId { get; }
        byte CurrencyId { get; }
        float Price { get; }
        DiscountType DiscountType { get; }
        float DiscountValue { get; }
        long DiscountTimeRemaining { get; }
        long Expiry { get; }

        float GetCurrencyValue();
    }
}