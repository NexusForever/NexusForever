using System.Collections.Immutable;
using NexusForever.Database;
using NexusForever.Database.World;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Storefront;
using NexusForever.Game.Static.Storefront;
using NexusForever.Network;
using NexusForever.Network.World.Message.Model;
using NexusForever.Shared;
using NLog;

namespace NexusForever.Game.Storefront
{
    /// <summary>
    /// GlobalStorefrontManager provides global caching of all the store items that are sent to each player. It was made global so that reloading store items while the server is 
    /// running would be handled in a global context.
    /// </summary>
    public sealed class GlobalStorefrontManager : Singleton<GlobalStorefrontManager>, IGlobalStorefrontManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private ImmutableDictionary<uint, ICategory> storeCategories;
        private ImmutableList<ServerStoreCategories.StoreCategory> serverStoreCategoryCache;

        private ImmutableDictionary<uint, IOfferGroup> offerGroups;
        private ImmutableList<ServerStoreOffers.OfferGroup> serverStoreOfferGroupCache;

        private ImmutableDictionary</*offerId*/uint, /*offerGroupId*/uint> offerGroupLookup;

        private GlobalStorefrontManager()
        {
        }

        public void Initialise()
        {
            InitialiseStoreCategories();
            InitialiseStoreOfferGroups();

            BuildNetworkPackets();

            log.Info($"Initialised {storeCategories.Count} categories with {offerGroups.Count} offers groups.");
        }

        private void InitialiseStoreCategories()
        {
            IEnumerable<StoreCategoryModel> storeCategoryModels = DatabaseManager.Instance.GetDatabase<WorldDatabase>().GetStoreCategories()
                .OrderBy(i => i.Id)
                .Where(x => x.ParentId != 0 // exclude top level parent category placeholder
                    && Convert.ToBoolean(x.Visible));

            var builder = ImmutableDictionary.CreateBuilder<uint, ICategory>();
            foreach (StoreCategoryModel category in storeCategoryModels)
                builder.Add(category.Id, new Category(category));

            storeCategories = builder.ToImmutable();
        }

        private void InitialiseStoreOfferGroups()
        {
            IEnumerable<StoreOfferGroupModel> offerGroupModels = DatabaseManager.Instance.GetDatabase<WorldDatabase>().GetStoreOfferGroups()
                .OrderBy(i => i.Id)
                .Where(x => Convert.ToBoolean(x.Visible));

            var offerGroupBuilder = ImmutableDictionary.CreateBuilder<uint, IOfferGroup>();
            var offerBuilder      = ImmutableDictionary.CreateBuilder<uint, uint>();
            foreach (StoreOfferGroupModel offerGroup in offerGroupModels)
            {
                offerGroupBuilder.Add(offerGroup.Id, new OfferGroup(offerGroup));

                foreach (StoreOfferItemModel offerItem in offerGroup.StoreOfferItem)
                    offerBuilder.Add(offerItem.Id, offerGroup.Id); // Cache the offer item's group ID, to lookup the entry.
            }
                
            offerGroups      = offerGroupBuilder.ToImmutable();
            offerGroupLookup = offerBuilder.ToImmutable();
        }

        private void BuildNetworkPackets()
        {
            var categoryBuilder = ImmutableList.CreateBuilder<ServerStoreCategories.StoreCategory>();
            foreach (ICategory category in storeCategories.Values)
                categoryBuilder.Add(category.Build());
            serverStoreCategoryCache = categoryBuilder.ToImmutable();

            var offerBuilder = ImmutableList.CreateBuilder<ServerStoreOffers.OfferGroup>();
            foreach (IOfferGroup offerGroup in offerGroups.Values)
                offerBuilder.Add(offerGroup.Build());
            serverStoreOfferGroupCache = offerBuilder.ToImmutable();
        }

        /// <summary>
        /// Return the <see cref="IOfferItem"/> that matches the supplied offer ID
        /// </summary>
        public IOfferItem GetStoreOfferItem(uint offerId)
        {
            if (!offerGroupLookup.TryGetValue(offerId, out uint offerGroupId))
                return null;

            return offerGroups.TryGetValue(offerGroupId, out IOfferGroup offerGroup) ? offerGroup.GetOfferItem(offerId) : null;
        }

        /// <summary>
        /// This method is used to send the current Store Catalog to the <see cref="IGameSession"/>
        /// </summary>
        public void HandleCatalogRequest(IGameSession session)
        {
            SendStoreCategories(session);
            SendStoreOffers(session);
            SendStoreFinalise(session);
        }

        private void SendStoreCategories(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerStoreCategories
            {
                StoreCategories = serverStoreCategoryCache.ToList(),
                RealCurrency    = RealCurrency.Usd
            });
        }

        private void SendStoreOffers(IGameSession session)
        {
            var storeOffers = new ServerStoreOffers();
            for (int i = 0; i < serverStoreOfferGroupCache.Count; i++)
            {
                storeOffers.OfferGroups.Add(serverStoreOfferGroupCache[i]);

                // Ensure we only send 20 Offer Groups per packet. This is the same as Live.
                if (i != 0 && i % 20 == 0
                    || i == serverStoreOfferGroupCache.Count - 1)
                {
                    session.EnqueueMessageEncrypted(storeOffers);
                    storeOffers.OfferGroups.Clear();
                }
            }
        }

        private void SendStoreFinalise(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerStoreFinalise());
        }
    }
}
