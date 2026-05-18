namespace NexusForever.Network.World.Message.Model.Shared
{
    // TODO: research this more, from what I can see these are the only 2 flags
    [Flags]
    public enum UpdateHealthMask
    {
        None            = 0x0000,
        FallDamage      = 0x0040,
        SuffocateDamage = 0x0200
    }
}
