using System.Collections.Generic;

namespace NexusForever.Database.World.Model
{
    public class StoreOfferGroupModel
    {
        public uint Id { get; set; }
        public uint DisplayFlags { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ushort DisplayInfoOverride { get; set; }
        public byte Visible { get; set; }

        public ICollection<StoreOfferGroupCategoryModel> StoreOfferGroupCategory { get; set; } = new HashSet<StoreOfferGroupCategoryModel>();
        public ICollection<StoreOfferItemModel> StoreOfferItem { get; set; } = new HashSet<StoreOfferItemModel>();
    }
}
