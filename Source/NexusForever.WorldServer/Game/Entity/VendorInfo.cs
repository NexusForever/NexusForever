using System.Collections.Immutable;
using System.Linq;
using NexusForever.WorldServer.Database.World.Model;
using EntityModel = NexusForever.WorldServer.Database.World.Model.Entity;

namespace NexusForever.WorldServer.Game.Entity
{
    public class VendorInfo
    {
        public uint Id { get; }
        public float SellPriceMultiplier { get; }
        public float BuyPriceMultiplier { get; }
        public ImmutableList<EntityVendorCategory> Categories { get; }
        public ImmutableList<EntityVendorItem> Items { get; }

        public VendorInfo(EntityModel model)
        {
            Id                  = model.EntityVendor.Id;
            SellPriceMultiplier = model.EntityVendor.SellPriceMultiplier;
            BuyPriceMultiplier  = model.EntityVendor.BuyPriceMultiplier;
            Categories          = model.EntityVendorCategory.ToImmutableList();
            Items               = model.EntityVendorItem.ToImmutableList();
        }

        /// <summary>
        /// Return <see cref="EntityVendorItem"/> at the supplied index;
        /// </summary>
        public EntityVendorItem GetItemAtIndex(uint index)
        {
            return Items.SingleOrDefault(i => i.Index == index);
        }
    }
}
