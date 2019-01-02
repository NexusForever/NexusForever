using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum TitleSaveMask
    {
        None          = 0x00,
        Create        = 0x01,
        TimeRemaining = 0x02,
        Revoked       = 0x04
    }
}
