namespace NexusForever.Game.Abstract.Entity
{
    public interface INonPlayer : IUnitEntity
    {
        IVendorInfo VendorInfo { get; }
    }
}