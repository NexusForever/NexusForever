using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="PathEntry"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum PathSaveMask
    {
        None               = 0x0000,
        Create             = 0x0001,
        Delete             = 0x0002,
        Change             = 0x0004,
        LevelUp            = 0x0008
    }
}
