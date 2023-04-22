using System.Collections.Immutable;
using NexusForever.Database.World.Model;
using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Entity
{
    public class VendorInfo : IVendorInfo
    {
        public uint Id { get; }
        public float SellPriceMultiplier { get; }
        public float BuyPriceMultiplier { get; }
        public ImmutableList<EntityVendorCategoryModel> Categories { get; }
        public ImmutableList<EntityVendorItemModel> Items { get; }

        public VendorInfo(EntityModel model)
        {
            Id                  = model.EntityVendor.Id;
            SellPriceMultiplier = model.EntityVendor.SellPriceMultiplier;
            BuyPriceMultiplier  = model.EntityVendor.BuyPriceMultiplier;
            Categories          = model.EntityVendorCategory.ToImmutableList();
            Items               = model.EntityVendorItem.ToImmutableList();
        }

        /// <summary>
        /// Return <see cref="EntityVendorItemModel"/> at the supplied index;
        /// </summary>
        public EntityVendorItemModel GetItemAtIndex(uint index)
        {
            return Items.SingleOrDefault(i => i.Index == index);
        }
    }
}
