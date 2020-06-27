namespace NexusForever.Database.World.Model
{
    public class StoreOfferGroupCategoryModel
    {
        public uint Id { get; set; }
        public uint CategoryId { get; set; }
        public byte Index { get; set; }
        public byte Visible { get; set; }

        public StoreCategoryModel Category { get; set; }
        public StoreOfferGroupModel OfferGroup { get; set; }
    }
}
