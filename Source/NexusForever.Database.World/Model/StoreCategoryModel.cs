using System.Collections.Generic;

namespace NexusForever.Database.World.Model
{
    public class StoreCategoryModel
    {
        public uint Id { get; set; }
        public uint ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint Index { get; set; }
        public byte Visible { get; set; }

        public ICollection<StoreOfferGroupCategoryModel> StoreOfferGroupCategory { get; set; } = new HashSet<StoreOfferGroupCategoryModel>();
    }
}
