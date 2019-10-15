namespace NexusForever.Database.World.Model
{
    public class EntityVendorModel
    {
        public uint Id { get; set; }
        public float BuyPriceMultiplier { get; set; }
        public float SellPriceMultiplier { get; set; }

        public EntityModel Entity { get; set; }
    }
}
