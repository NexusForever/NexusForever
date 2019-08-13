using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class StoreCategory
    {
        public StoreCategory()
        {
            StoreOfferGroupCategory = new HashSet<StoreOfferGroupCategory>();
        }

        public uint Id { get; set; }
        public uint ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public uint Index { get; set; }
        public byte Visible { get; set; }

        public virtual ICollection<StoreOfferGroupCategory> StoreOfferGroupCategory { get; set; }
    }
}
