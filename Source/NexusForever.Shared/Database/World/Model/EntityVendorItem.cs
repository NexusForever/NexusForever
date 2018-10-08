using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.World.Model
{
    public partial class EntityVendorItem
    {
        public uint Id { get; set; }
        public uint Index { get; set; }
        public uint CategoryIndex { get; set; }
        public uint ItemId { get; set; }

        public Entity IdNavigation { get; set; }
    }
}
