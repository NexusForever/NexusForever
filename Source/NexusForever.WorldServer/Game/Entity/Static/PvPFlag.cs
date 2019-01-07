using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Enum containing properties used to set the <see cref="Player"/> PvP Flag
    /// </summary>
    [Flags]
    public enum PvPFlag
    {
        Disabled    = 0x00,
        Enabled     = 0x01,
        Forced      = 0x02  // Disables "Turn On/Off PvP" in Portrait.
    }
}
