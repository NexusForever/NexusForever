namespace NexusForever.Game.Abstract.Entity
{
    public interface INonPlayerEntity : ICreatureEntity
    {
        IVendorInfo VendorInfo { get; }
    }
}
