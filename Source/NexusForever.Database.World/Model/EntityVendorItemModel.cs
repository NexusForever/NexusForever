using NexusForever.Game.Static.Entity;

namespace NexusForever.Database.World.Model
{
    public class EntityVendorItemModel
    {
        public uint Id { get; set; }
        public uint Index { get; set; }
        public uint CategoryIndex { get; set; }
        public uint ItemId { get; set; }
        public ItemExtraCostType ExtraCost1Type { get; set; }
        public uint ExtraCost1Quantity { get; set; }
        public uint ExtraCost1ItemOrCurrencyId { get; set; }
        public ItemExtraCostType ExtraCost2Type { get; set; }
        public uint ExtraCost2Quantity { get; set; }
        public uint ExtraCost2ItemOrCurrencyId { get; set; }

        public EntityModel Entity { get; set; }
    }
}
