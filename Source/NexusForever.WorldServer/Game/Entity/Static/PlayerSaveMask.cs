﻿using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Player"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum PlayerSaveMask
    {
        None      = 0x0000,
        Level     = 0x0001,
        Location  = 0x0002,
        Path      = 0x0004
    }
}
