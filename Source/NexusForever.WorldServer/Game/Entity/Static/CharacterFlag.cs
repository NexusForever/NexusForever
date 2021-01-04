using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum CharacterFlag
    {
        None               = 0x0000,
        IgnoreDuelRequests = 0x0008,
        HolomarkHideLeft   = 0x0020,
        HolomarkHideRight  = 0x0040,
        HolomarkHideBack   = 0x0080,
        HolomarkNear       = 0x0200
    }
}
