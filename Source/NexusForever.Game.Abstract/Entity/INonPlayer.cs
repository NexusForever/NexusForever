namespace NexusForever.Game.Abstract.Entity
{
    public interface INonPlayer : ICreatureEntity
    {
        IVendorInfo VendorInfo { get; }
    }
}