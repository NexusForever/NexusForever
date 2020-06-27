using System.Collections.Generic;

namespace NexusForever.Database.World.Model
{
    public class StoreOfferItemModel
    {
        public uint Id { get; set; }
        public uint GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint DisplayFlags { get; set; }
        public long Field6 { get; set; }
        public byte Field7 { get; set; }
        public byte Visible { get; set; }

        public StoreOfferGroupModel Group { get; set; }
        public ICollection<StoreOfferItemDataModel> StoreOfferItemData { get; set; } = new HashSet<StoreOfferItemDataModel>();
        public ICollection<StoreOfferItemPriceModel> StoreOfferItemPrice { get; set; } = new HashSet<StoreOfferItemPriceModel>();
    }
}
