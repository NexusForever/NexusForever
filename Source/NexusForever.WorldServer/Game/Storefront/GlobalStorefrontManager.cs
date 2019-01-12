using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Shared.Configuration;
using NexusForever.WorldServer.Database.World;
using NexusForever.WorldServer.Database.World.Model;
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

        private static ImmutableDictionary<uint, Category> StoreCategoryCache { get; set; }
        private static List<ServerStoreCategories.StoreCategory> ServerStoreCategoryList { get; set; } = new List<ServerStoreCategories.StoreCategory>();

        private static ImmutableDictionary<uint, OfferGroup> StoreOfferCache { get; set; }
        private static List<ServerStoreOffers.OfferGroup> ServerStoreOfferList { get; set; } = new List<ServerStoreOffers.OfferGroup>();

        private static ImmutableDictionary</*offerId*/uint, /*offerGroupId*/uint> OfferItemLookupCache { get; set; }

        public static float ForcedProtobucksPrice { get; private set; } = 0f;
        public static float ForcedOmnibitsPrice { get; private set; } = 0f;
        public static bool CurrencyProtobucksEnabled { get; private set; } = true;
        public static bool CurrencyOmnibitsEnabled { get; private set; } = true;

        public static void Initialise()
        {
            LoadConfig();

            InitialiseStoreCategories();
            InitialiseStoreOfferGroups();

            BuildNetworkPackets();

            log.Info($"Initialised {StoreCategoryCache.Count} categories with {StoreOfferCache.Count} offers");
        }

        private static void LoadConfig()
        {
            ForcedProtobucksPrice = ConfigurationManager<WorldServerConfiguration>.Config.Store.ForcedProtobucksPrice;
            ForcedOmnibitsPrice = ConfigurationManager<WorldServerConfiguration>.Config.Store.ForcedOmnibitsPrice;
            CurrencyProtobucksEnabled = ConfigurationManager<WorldServerConfiguration>.Config.Store.CurrencyProtobucksEnabled;
            CurrencyOmnibitsEnabled = ConfigurationManager<WorldServerConfiguration>.Config.Store.CurrencyOmnibitsEnabled;
        }

        private static void BuildNetworkPackets()
        {
            foreach (Category category in StoreCategoryCache.Values)
                ServerStoreCategoryList.Add(category.BuildNetworkPacket());

            foreach (OfferGroup offerGroup in StoreOfferCache.Values)
                ServerStoreOfferList.Add(offerGroup.BuildNetworkPacket());
        }

        private static void InitialiseStoreCategories()
        {
            IEnumerable<StoreCategory> StoreCategories = WorldDatabase.GetStoreCategories()
                .OrderBy(i => i.Id)
                .Where(x => x.Id != 26 && Convert.ToBoolean(x.Visible) == true); // Remove parent category placeholder

            var categoryList = ImmutableDictionary.CreateBuilder<uint, Category>();
            foreach (StoreCategory category in StoreCategories)
                categoryList.Add(category.Id, new Category(category));

            StoreCategoryCache = categoryList.ToImmutable();
        }

        private static void InitialiseStoreOfferGroups()
        {
            IEnumerable<StoreOfferGroup> StoreOfferGroups = WorldDatabase.GetStoreOfferGroups()
                .OrderBy(i => i.Id)
                .Where(x => Convert.ToBoolean(x.Visible) == true);

            var offerGroupList = ImmutableDictionary.CreateBuilder<uint, OfferGroup>();
            var offerItems = ImmutableDictionary.CreateBuilder<uint, uint>();

            foreach (StoreOfferGroup offerGroup in StoreOfferGroups)
            {
                offerGroupList.Add(offerGroup.Id, new OfferGroup(offerGroup));

                foreach (var offerItem in offerGroup.StoreOfferItem)
                    offerItems.Add(offerItem.Id, offerGroup.Id); // Cache the offer item's group ID, to lookup the entry.
            }
                
            StoreOfferCache = offerGroupList.ToImmutable();
            OfferItemLookupCache = offerItems.ToImmutable();
        }

        /// <summary>
        /// Return the <see cref="OfferItem"/> that matches the supplied offer ID
        /// </summary>
        public static OfferItem GetStoreOfferItem(uint offerId)
        {
            if(OfferItemLookupCache.TryGetValue(offerId, out uint offerGroupId))
            {
                if (StoreOfferCache.TryGetValue(offerGroupId, out OfferGroup offerGroup))
                {
                    return offerGroup.GetOfferItem(offerId);
                }
            }

            return null;
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
            ServerStoreCategories serverCategories = new ServerStoreCategories
            {
                StoreCategories = ServerStoreCategoryList.ToList(),
                Unknown4 = 4
            };

            session.EnqueueMessageEncrypted(serverCategories);
        }

        private static void SendStoreOffers(WorldSession session)
        {
            List<ServerStoreOffers.OfferGroup> offersToSend = new List<ServerStoreOffers.OfferGroup>();
            uint count = 0;

            foreach(ServerStoreOffers.OfferGroup offerGroup in ServerStoreOfferList)
            {
                count++;
                offersToSend.Add(offerGroup);
                // Ensure we only send 20 Offer Groups per packet. This is the same as Live.
                if(count == 20 || ServerStoreOfferList.IndexOf(offerGroup) == ServerStoreOfferList.Count - 1)
                {
                    session.EnqueueMessageEncrypted(new ServerStoreOffers
                    {
                        OfferGroups = offersToSend
                    });
                    offersToSend.RemoveRange(0, (int)count);
                    count = 0;
                }
            }
        }

        private static void SendStoreFinalise(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerStoreFinalise());
        }
    }
}
