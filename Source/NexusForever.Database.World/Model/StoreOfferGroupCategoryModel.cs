namespace NexusForever.Database.World.Model
{
    public class StoreOfferGroupCategoryModel
    {
        public uint Id { get; set; }
        public uint CategoryId { get; set; }
        public byte Index { get; set; }
        public byte Visible { get; set; }

        public virtual StoreOfferGroupModel OfferGroup { get; set; }
        public virtual StoreCategoryModel Category { get; set; }
    }
}
