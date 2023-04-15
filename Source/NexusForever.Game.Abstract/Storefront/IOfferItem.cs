using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Storefront;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Abstract.Storefront
{
    public interface IOfferItem : INetworkBuildable<ServerStoreOffers.OfferGroup.Offer>
    {
        uint Id { get; }
        string Name { get; }
        string Description { get; }
        DisplayFlag DisplayFlags { get; }
        long Field6 { get; }
        byte Field7 { get; }
        bool Visible { get; }

        /// <summary>
        /// Get the <see cref="IOfferItemPrice"/> associated with this <see cref="IOfferItem"/> for the given account currency ID
        /// </summary>
        IOfferItemPrice GetPriceDataForCurrency(AccountCurrencyType currencyId);
    }
}