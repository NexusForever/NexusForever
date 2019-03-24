using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum AmpSaveMask
    {
        None    = 0x0000,
        Create  = 0x0001,
        Delete  = 0x0002
    }
}
