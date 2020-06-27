namespace NexusForever.Database.World.Model
{
    public class StoreOfferItemDataModel
    {
        public uint Id { get; set; }
        public ushort ItemId { get; set; }
        public uint Type { get; set; }
        public uint Amount { get; set; }

        public StoreOfferItemModel OfferItem { get; set; }
    }
}
