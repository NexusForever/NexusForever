using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Player"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum PlayerSaveMask
    {
        None        = 0x0000,
        Location    = 0x0001,
        Path        = 0x0002,
        Costume     = 0x0004,
        InputKeySet = 0x0008,
        Xp          = 0x0010,
        Flags       = 0x0020,
        Innate      = 0x0080
    }
}
