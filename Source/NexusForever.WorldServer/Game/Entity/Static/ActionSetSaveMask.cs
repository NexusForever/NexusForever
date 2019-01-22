using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum ActionSetSaveMask
    {
        None            = 0x0000,
        ActionSetActions= 0x0001,
        ActionSetAMPs   = 0x0002
    }
}
