using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum StatSaveMask
    {
        None   = 0x00,
        Create = 0x01,
        Value  = 0x02
    }
}
