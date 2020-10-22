using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum EntityStatus
    {
        None        = 0x0000,
        Stealth     = 0x0001
    }
}
