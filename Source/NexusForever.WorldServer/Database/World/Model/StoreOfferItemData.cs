using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class StoreOfferItemData
    {
        public uint Id { get; set; }
        public ushort ItemId { get; set; }
        public uint Type { get; set; }
        public uint Amount { get; set; }

        public virtual StoreOfferItem IdNavigation { get; set; }
    }
}
