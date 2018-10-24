using System.Collections.Immutable;
using NexusForever.WorldServer.Database.World.Model;

namespace NexusForever.WorldServer.Game.Entity
{
    public class VendorInfo
    {
        public uint Id { get; }
        public float SellPriceMultiplier { get; }
        public float BuyPriceMultiplier { get; }
        public ImmutableList<EntityVendorCategory> Categories { get; }
        public ImmutableList<EntityVendorItem> Items { get; }

        public VendorInfo(EntityVendor vendor, ImmutableList<EntityVendorCategory> categories, ImmutableList<EntityVendorItem> items)
        {
            Id                  = vendor.Id;
            SellPriceMultiplier = vendor.SellPriceMultiplier;
            BuyPriceMultiplier  = vendor.BuyPriceMultiplier;
            Categories          = categories;
            Items               = items;
        }
    }
}
