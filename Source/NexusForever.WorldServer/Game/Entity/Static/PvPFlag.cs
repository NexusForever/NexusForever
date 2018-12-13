using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Player"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum PvPFlag
    {
        Disabled    = 0,
        Enabled     = 1,
        Forced      = 2, // Disables "Turn On/Off PvP" in Portrait.
    }
}
