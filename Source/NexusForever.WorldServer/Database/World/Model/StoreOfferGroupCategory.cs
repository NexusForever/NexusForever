using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class StoreOfferGroupCategory
    {
        public uint Id { get; set; }
        public uint CategoryId { get; set; }
        public byte Index { get; set; }
        public byte Visible { get; set; }

        public virtual StoreCategory Category { get; set; }
        public virtual StoreOfferGroup IdNavigation { get; set; }
    }
}
