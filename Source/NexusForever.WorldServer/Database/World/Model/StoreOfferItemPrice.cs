using System;
using System.Collections.Generic;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class StoreOfferItemPrice
    {
        public uint Id { get; set; }
        public byte CurrencyId { get; set; }
        public float Price { get; set; }
        public byte DiscountType { get; set; }
        public float DiscountValue { get; set; }
        public long Field14 { get; set; }
        public long Expiry { get; set; }

        public virtual StoreOfferItem IdNavigation { get; set; }
    }
}
