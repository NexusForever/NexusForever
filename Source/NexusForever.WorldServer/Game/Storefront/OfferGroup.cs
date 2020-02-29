using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Database.World.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Storefront.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Game.Storefront
{
    public class OfferGroup : IBuildable<ServerStoreOffers.OfferGroup>
    {
        public struct Category
        {
            public uint Id { get; set; }
            public uint Index { get; set; }
        }

        public uint Id { get; }
        public DisplayFlag DisplayFlags { get; }
        public string Name { get; }
        public string Description { get; }
        public ushort DisplayInfoOverride { get; }
        public bool Visible { get; }
        public ImmutableList<Category> Categories { get; }

        private readonly Dictionary</*offerId*/uint, OfferItem> offerItems = new Dictionary<uint, OfferItem>();

        /// <summary>
        /// Create a new <see cref="OfferGroup"/> from an existing database model.
        /// </summary>
        public OfferGroup(StoreOfferGroupModel model)
        {
            Id           = model.Id;
            DisplayFlags = (DisplayFlag)model.DisplayFlags;
            Name         = model.Name;
            Description  = model.Description;
            DisplayInfoOverride = model.DisplayInfoOverride;
            Visible      = Convert.ToBoolean(model.Visible);

            var builder = ImmutableList.CreateBuilder<Category>();
            foreach (StoreOfferGroupCategoryModel categoryModel in model.StoreOfferGroupCategory)
            {
                if (Convert.ToBoolean(categoryModel.Visible))
                {
                    builder.Add(new Category
                    {
                        Id    = categoryModel.CategoryId,
                        Index = categoryModel.Index
                    });
                }
            }

            Categories = builder.ToImmutable();

            foreach (StoreOfferItemModel offerItem in model.StoreOfferItem)
            {
                var offer = new OfferItem(offerItem);
                offerItems.Add(offer.Id, offer);
            }
        }

        /// <summary>
        /// Returns an <see cref="OfferItem"/> matching the supplied offer ID, if it exists
        /// </summary>
        public OfferItem GetOfferItem(uint offerId)
        {
            return offerItems.TryGetValue(offerId, out OfferItem offerItem) ? offerItem : null;
        }

        public ServerStoreOffers.OfferGroup Build()
        {
            return new ServerStoreOffers.OfferGroup
            {
                Id                  = Id,
                DisplayFlags        = DisplayFlags,
                Name                = Name,
                Description         = Description,
                DisplayInfoOverride = DisplayInfoOverride,
                Categories          = Categories
                    .Select(c => new ServerStoreOffers.OfferGroup.Category
                    {
                        Id    = c.Id,
                        Index = c.Index
                    })
                    .ToList(),
                Offers = offerItems.Values
                    .Select(i => i.Build())
                    .ToList()
            };
        }
    }
}
