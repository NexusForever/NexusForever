using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum TitleSaveMask
    {
        None          = 0x00,
        TimeRemaining = 0x01
    }
}
