using System;

namespace NexusForever.WorldServer.Network.Message.Model.Shared
{
    // TODO: research this more, from what I can see these are the only 2 flags
    [Flags]
    public enum UpdateHealthMask
    {
        FallDamage      = 0x0040,
        SuffocateDamage = 0x0200
    }
}
