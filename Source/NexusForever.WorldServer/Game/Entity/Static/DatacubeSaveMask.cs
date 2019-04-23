using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum DatacubeSaveMask
    {
        None    = 0x0000,
        Create  = 0x0001,
        Modify  = 0x0002,
    }
}
