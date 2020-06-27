namespace NexusForever.Database.World.Model
{
    public class EntityVendorCategoryModel
    {
        public uint Id { get; set; }
        public uint Index { get; set; }
        public uint LocalisedTextId { get; set; }

        public EntityModel Entity { get; set; }
    }
}
