using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum StatSaveMask
    {
        None          = 0x00,
        Modified      = 0x01,
        Create        = 0x02,
    }
}
