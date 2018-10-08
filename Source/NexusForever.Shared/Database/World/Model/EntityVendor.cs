using System;
using System.Collections.Generic;

namespace NexusForever.Shared.Database.World.Model
{
    public partial class EntityVendor
    {
        public uint Id { get; set; }
        public float BuyPriceMultiplier { get; set; }
        public float SellPriceMultiplier { get; set; }

        public Entity IdNavigation { get; set; }
    }
}
