using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum ActionSaveMask
    {
        None    = 0x0000,
        Create  = 0x0001,
        Delete  = 0x0002,
        Modify  = 0x0004
    }
}
