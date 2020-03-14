using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class EntityVendor
    {
        public uint Id { get; set; }
        public float BuyPriceMultiplier { get; set; }
        public float SellPriceMultiplier { get; set; }

        public virtual Entity IdNavigation { get; set; }
    }
}
