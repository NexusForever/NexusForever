using System;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Database.World.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Game.Static;
using NexusForever.WorldServer.Game.Storefront.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class OfferItem : IBuildable<ServerStoreOffers.OfferGroup.Offer>
    {
        public uint Id { get; }
        public string Name { get; }
        public string Description { get; }
        public DisplayFlag DisplayFlags { get; }
        public long Field6 { get; }
        public byte Field7 { get; }
        public bool Visible { get; }

        private readonly ImmutableList<OfferItemData> items;
        private readonly ImmutableDictionary<AccountCurrencyType, OfferItemPrice> prices;

        /// <summary>
        /// Create a new <see cref="OfferItem"/> from an existing database model.
        /// </summary>
        public OfferItem(StoreOfferItemModel model)
        {
            Id           = model.Id;
            Name         = model.Name;
            Description  = model.Description;
            DisplayFlags = (DisplayFlag)model.DisplayFlags;
            Field6       = model.Field6;
            Field7       = model.Field7;
            Visible      = Convert.ToBoolean(model.Visible);

            var itemBuilder = ImmutableList.CreateBuilder<OfferItemData>();
            foreach (StoreOfferItemDataModel itemData in model.StoreOfferItemData)
                itemBuilder.Add(new OfferItemData(itemData));

            items = itemBuilder.ToImmutable();

            var priceBuilder = ImmutableDictionary.CreateBuilder<AccountCurrencyType, OfferItemPrice>();
            foreach (StoreOfferItemPriceModel price in model.StoreOfferItemPrice)
            {
                if (DisableManager.Instance.IsDisabled(DisableType.AccountCurrency, price.CurrencyId))
                    continue;

                var itemPrice = new OfferItemPrice(price);
                priceBuilder.Add((AccountCurrencyType)itemPrice.CurrencyId, itemPrice);
            }

            prices = priceBuilder.ToImmutable();
        }

        /// <summary>
        /// Get the <see cref="OfferItemPrice"/> associated with this <see cref="OfferItem"/> for the given account currency ID
        /// </summary>
        public OfferItemPrice GetPriceDataForCurrency(AccountCurrencyType currencyId)
        {
            return prices.TryGetValue(currencyId, out OfferItemPrice itemPrice) ? itemPrice : null;
        }

        public ServerStoreOffers.OfferGroup.Offer Build()
        {
            float pricePremium = 0f;
            float priceAlternative = 0;

            if (prices.TryGetValue(AccountCurrencyType.Protobuck, out OfferItemPrice protobucksItemPrice))
                pricePremium = protobucksItemPrice.GetCurrencyValue();
            if (prices.TryGetValue(AccountCurrencyType.Omnibit, out OfferItemPrice omnibitsItemPrice))
                priceAlternative = omnibitsItemPrice.GetCurrencyValue();

            return new ServerStoreOffers.OfferGroup.Offer
            {
                Id               = Id,
                Name             = Name,
                Description      = Description,
                DisplayFlags     = DisplayFlags,
                PricePremium     = pricePremium,
                PriceAlternative = priceAlternative,
                Unknown6         = Field6,
                Unknown7         = Field7,
                ItemData         = items
                    .Select(i => i.Build())
                    .ToList(),
                CurrencyData = prices.Values
                    .Select(p => p.Build())
                    .ToList()
            };
        }
    }
}
