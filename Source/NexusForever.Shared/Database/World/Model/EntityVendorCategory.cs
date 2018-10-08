using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.World.Model
{
    public partial class EntityVendorCategory
    {
        public uint Id { get; set; }
        public uint Index { get; set; }
        public uint LocalisedTextId { get; set; }

        public Entity IdNavigation { get; set; }
    }
}
