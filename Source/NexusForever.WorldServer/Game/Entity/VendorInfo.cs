using System.Collections.Immutable;
using System.Linq;
using NexusForever.Database.World.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class VendorInfo
    {
        public uint Id { get; }
        public float SellPriceMultiplier { get; }
        public float BuyPriceMultiplier { get; }
        public ImmutableList<EntityVendorCategoryModel> Categories { get; }
        public ImmutableList<EntityVendorItemModel> Items { get; }

        public VendorInfo(EntityModel model)
        {
            Id                  = model.Vendor.Id;
            SellPriceMultiplier = model.Vendor.SellPriceMultiplier;
            BuyPriceMultiplier  = model.Vendor.BuyPriceMultiplier;
            Categories          = model.VendorCategories.ToImmutableList();
            Items               = model.VendorItems.ToImmutableList();
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
