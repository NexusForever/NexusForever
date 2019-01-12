using NexusForever.WorldServer.Database.World.Model;
using NexusForever.WorldServer.Game.Storefront.Static;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class OfferItem
    {
        public uint Id { get; }
        public string Name { get; }
        public string Description { get; }
        public DisplayFlag DisplayFlags { get; }
        public long Field6 { get; }
        public byte Field7 { get; }
        public bool Visible { get; }

        private List<OfferItemData> ItemDataList { get; set; } = new List<OfferItemData>();
        private Dictionary<byte, OfferItemPrice> Prices { get; set; } = new Dictionary<byte, OfferItemPrice>();

        public OfferItem(StoreOfferItem model)
        {
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            DisplayFlags = (DisplayFlag)model.DisplayFlags;
            Field6 = model.Field6;
            Field7 = model.Field7;
            Visible = Convert.ToBoolean(model.Visible);

            foreach (StoreOfferItemData itemData in model.StoreOfferItemData)
                ItemDataList.Add(new OfferItemData(itemData));
                
            foreach (StoreOfferItemPrice price in model.StoreOfferItemPrice)
            {
                if (!GlobalStorefrontManager.CurrencyProtobucksEnabled && price.CurrencyId == 11)
                    continue;
                if (!GlobalStorefrontManager.CurrencyOmnibitsEnabled && price.CurrencyId == 6)
                    continue;

                OfferItemPrice itemPrice = new OfferItemPrice(price);
                Prices.Add(itemPrice.CurrencyId, itemPrice);
            }
        }

        private IEnumerable<ServerStoreOffers.OfferGroup.Offer.OfferItemData> GetItemNetworkPackets()
        {
            foreach (OfferItemData item in ItemDataList)
                yield return item.BuildNetworkPacket();
        }

        private IEnumerable<ServerStoreOffers.OfferGroup.Offer.OfferCurrencyData> GetPricingNetworkPackets()
        {
            foreach (OfferItemPrice price in Prices.Values)
                yield return price.BuildNetworkPacket();
        }

        /// <summary>
        /// Get all <see cref="OfferItemData"/> as part of this <see cref="OfferItem"/>
        /// </summary>
        public IEnumerable<OfferItemData> GetOfferItems()
        {
            return ItemDataList;
        }

        /// <summary>
        /// Get the <see cref="OfferItemPrice"/> associated with this <see cref="OfferItem"/> for the given account currency ID
        /// </summary>
        public OfferItemPrice GetPriceDataForCurrency(byte currencyId)
        {
            return Prices.TryGetValue(currencyId, out OfferItemPrice itemPrice) ? itemPrice : null;
        }

        public ServerStoreOffers.OfferGroup.Offer BuildNetworkPacket()
        {
            float priceProtobucks = 0;
            float priceOmnibits = 0;

            if (Prices.TryGetValue(11, out OfferItemPrice protobucksItemPrice))
                priceProtobucks = protobucksItemPrice.GetCurrencyValue();
            if (Prices.TryGetValue(6, out OfferItemPrice omnibitsItemPrice))
                priceOmnibits = omnibitsItemPrice.GetCurrencyValue();

            return new ServerStoreOffers.OfferGroup.Offer
            {
                Id = Id,
                OfferName = Name,
                OfferDescription = Description,
                DisplayFlags = DisplayFlags,
                PriceProtobucks = priceProtobucks,
                PriceOmnibits = priceOmnibits,
                Unknown6 = Field6,
                Unknown7 = Field7,
                ItemData = GetItemNetworkPackets().ToList(),
                CurrencyData = GetPricingNetworkPackets().ToList()
            };
        }
    }
}
