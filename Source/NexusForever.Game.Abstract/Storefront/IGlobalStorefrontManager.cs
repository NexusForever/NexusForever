using NexusForever.Network.Session;

namespace NexusForever.Game.Abstract.Storefront
{
    public interface IGlobalStorefrontManager
    {
        void Initialise();

        /// <summary>
        /// Return the <see cref="IOfferItem"/> that matches the supplied offer ID
        /// </summary>
        IOfferItem GetStoreOfferItem(uint offerId);

        /// <summary>
        /// This method is used to send the current Store Catalog to the <see cref="IGameSession"/>
        /// </summary>
        void HandleCatalogRequest(IGameSession session);
    }
}