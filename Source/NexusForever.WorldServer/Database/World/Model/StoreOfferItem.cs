using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class StoreOfferItem
    {
        public StoreOfferItem()
        {
            StoreOfferItemData = new HashSet<StoreOfferItemData>();
            StoreOfferItemPrice = new HashSet<StoreOfferItemPrice>();
        }

        public uint Id { get; set; }
        public uint GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint DisplayFlags { get; set; }
        public long Field6 { get; set; }
        public byte Field7 { get; set; }
        public byte Visible { get; set; }

        public virtual StoreOfferGroup Group { get; set; }
        public virtual ICollection<StoreOfferItemData> StoreOfferItemData { get; set; }
        public virtual ICollection<StoreOfferItemPrice> StoreOfferItemPrice { get; set; }
    }
}
