using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Database.World.Model;
using NexusForever.Shared.Database;
using NexusForever.WorldServer.Game.Storefront.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Game.Storefront
{
    /// <summary>
    /// GlobalStorefrontManager provides global caching of all the store items that are sent to each player. It was made global so that reloading store items while the server is 
    /// running would be handled in a global context.
    /// </summary>
    public static class GlobalStorefrontManager
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static ImmutableDictionary<uint, Category> storeCategories;
        private static ImmutableList<ServerStoreCategories.StoreCategory> serverStoreCategoryCache;

        private static ImmutableDictionary<uint, OfferGroup> offerGroups;
        private static ImmutableList<ServerStoreOffers.OfferGroup> serverStoreOfferGroupCache;

        private static ImmutableDictionary</*offerId*/uint, /*offerGroupId*/uint> offerGroupLookup;

        public static void Initialise()
        {
            InitialiseStoreCategories();
            InitialiseStoreOfferGroups();

            BuildNetworkPackets();

            log.Info($"Initialised {storeCategories.Count} categories with {offerGroups.Count} offers groups.");
        }

        private static void InitialiseStoreCategories()
        {
            IEnumerable<StoreCategoryModel> storeCategoryModels = DatabaseManager.WorldDatabase.GetStoreCategories()
                .OrderBy(i => i.Id)
                .Where(x => x.ParentId != 0 // exclude top level parent category placeholder
                    && Convert.ToBoolean(x.Visible));

            var builder = ImmutableDictionary.CreateBuilder<uint, Category>();
            foreach (StoreCategoryModel category in storeCategoryModels)
                builder.Add(category.Id, new Category(category));

            storeCategories = builder.ToImmutable();
        }

        private static void InitialiseStoreOfferGroups()
        {
            IEnumerable<StoreOfferGroupModel> offerGroupModels = DatabaseManager.WorldDatabase.GetStoreOfferGroups()
                .OrderBy(i => i.Id)
                .Where(x => Convert.ToBoolean(x.Visible));

            var offerGroupBuilder = ImmutableDictionary.CreateBuilder<uint, OfferGroup>();
            var offerBuilder      = ImmutableDictionary.CreateBuilder<uint, uint>();
            foreach (StoreOfferGroupModel offerGroup in offerGroupModels)
            {
                offerGroupBuilder.Add(offerGroup.Id, new OfferGroup(offerGroup));

                foreach (StoreOfferItemModel offerItem in offerGroup.Items)
                    offerBuilder.Add(offerItem.Id, offerGroup.Id); // Cache the offer item's group ID, to lookup the entry.
            }
                
            offerGroups      = offerGroupBuilder.ToImmutable();
            offerGroupLookup = offerBuilder.ToImmutable();
        }

        private static void BuildNetworkPackets()
        {
            var categoryBuilder = ImmutableList.CreateBuilder<ServerStoreCategories.StoreCategory>();
            foreach (Category category in storeCategories.Values)
                categoryBuilder.Add(category.BuildNetworkPacket());
            serverStoreCategoryCache = categoryBuilder.ToImmutable();

            var offerBuilder = ImmutableList.CreateBuilder<ServerStoreOffers.OfferGroup>();
            foreach (OfferGroup offerGroup in offerGroups.Values)
                offerBuilder.Add(offerGroup.BuildNetworkPacket());
            serverStoreOfferGroupCache = offerBuilder.ToImmutable();
        }

        /// <summary>
        /// Return the <see cref="OfferItem"/> that matches the supplied offer ID
        /// </summary>
        public static OfferItem GetStoreOfferItem(uint offerId)
        {
            if (!offerGroupLookup.TryGetValue(offerId, out uint offerGroupId))
                return null;

            return offerGroups.TryGetValue(offerGroupId, out OfferGroup offerGroup) ? offerGroup.GetOfferItem(offerId) : null;
        }

        /// <summary>
        /// This method is used to send the current Store Catalog to the <see cref="WorldSession"/>
        /// </summary>
        public static void HandleCatalogRequest(WorldSession session)
        {
            SendStoreCategories(session);
            SendStoreOffers(session);
            SendStoreFinalise(session);
        }

        private static void SendStoreCategories(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerStoreCategories
            {
                StoreCategories = serverStoreCategoryCache.ToList(),
                RealCurrency    = RealCurrency.Usd
            });
        }

        private static void SendStoreOffers(WorldSession session)
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

        private static void SendStoreFinalise(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerStoreFinalise());
        }
    }
}
