using System;

namespace NexusForever.WorldServer.Game.Storefront.Static
{
    [Flags]
    public enum DisplayFlag
    {
        None        = 0x00,
        New         = 0x01,
        Recommended = 0x02,
        Popular     = 0x04,
        LimitedTime = 0x08
    }
}
