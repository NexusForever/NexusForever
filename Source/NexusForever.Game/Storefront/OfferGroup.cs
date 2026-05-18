using System.Collections.Immutable;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Storefront;
using NexusForever.Game.Static.Storefront;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Storefront
{
    public class OfferGroup : IOfferGroup
    {
        public uint Id { get; }
        public DisplayFlag DisplayFlags { get; }
        public string Name { get; }
        public string Description { get; }
        public ushort DisplayInfoOverride { get; }
        public bool Visible { get; }
        public ImmutableList<IOfferGroupCategory> Categories { get; }

        private readonly Dictionary</*offerId*/uint, IOfferItem> offerItems = new();

        /// <summary>
        /// Create a new <see cref="IOfferGroup"/> from an existing database model.
        /// </summary>
        public OfferGroup(StoreOfferGroupModel model)
        {
            Id           = model.Id;
            DisplayFlags = (DisplayFlag)model.DisplayFlags;
            Name         = model.Name;
            Description  = model.Description;
            DisplayInfoOverride = model.DisplayInfoOverride;
            Visible      = Convert.ToBoolean(model.Visible);

            var builder = ImmutableList.CreateBuilder<IOfferGroupCategory>();
            foreach (StoreOfferGroupCategoryModel categoryModel in model.StoreOfferGroupCategory)
            {
                if (Convert.ToBoolean(categoryModel.Visible))
                {
                    builder.Add(new OfferGroupCategory
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
        /// Returns an <see cref="IOfferItem"/> matching the supplied offer ID, if it exists.
        /// </summary>
        public IOfferItem GetOfferItem(uint offerId)
        {
            return offerItems.TryGetValue(offerId, out IOfferItem offerItem) ? offerItem : null;
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
