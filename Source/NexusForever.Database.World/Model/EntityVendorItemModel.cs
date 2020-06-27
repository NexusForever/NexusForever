namespace NexusForever.Database.World.Model
{
    public class EntityVendorItemModel
    {
        public uint Id { get; set; }
        public uint Index { get; set; }
        public uint CategoryIndex { get; set; }
        public uint ItemId { get; set; }

        public EntityModel Entity { get; set; }
    }
}
