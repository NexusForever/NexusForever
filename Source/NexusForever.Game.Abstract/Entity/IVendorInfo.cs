using System.Collections.Immutable;
using NexusForever.Database.World.Model;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IVendorInfo
    {
        uint Id { get; }
        float SellPriceMultiplier { get; }
        float BuyPriceMultiplier { get; }
        ImmutableList<EntityVendorCategoryModel> Categories { get; }
        ImmutableList<EntityVendorItemModel> Items { get; }
        
        /// <summary>
        /// Return <see cref="EntityVendorItemModel"/> at the supplied index;
        /// </summary>
        EntityVendorItemModel GetItemAtIndex(uint index);
    }
}