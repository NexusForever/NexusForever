using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="PathEntry"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum PathSaveMask
    {
        None                = 0x0000,
        Create              = 0x0001,
        Unlocked            = 0x0002,
        XPChange            = 0x0004,
        LevelChange         = 0x0008
    }
}
