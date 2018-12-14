using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Enum containing properties used to set the <see cref="Player"/> PvP Flag
    /// </summary>
    [Flags]
    public enum PvPFlag
    {
        Disabled    = 0,
        Enabled     = 1,
        Forced      = 2, // Disables "Turn On/Off PvP" in Portrait.
    }
}
