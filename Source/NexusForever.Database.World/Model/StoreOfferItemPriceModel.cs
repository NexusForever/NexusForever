namespace NexusForever.Database.World.Model
{
    public class StoreOfferItemPriceModel
    {
        public uint Id { get; set; }
        public byte CurrencyId { get; set; }
        public float Price { get; set; }
        public byte DiscountType { get; set; }
        public float DiscountValue { get; set; }
        public long Field14 { get; set; }
        public long Expiry { get; set; }

        public StoreOfferItemModel OfferItem { get; set; }
    }
}
