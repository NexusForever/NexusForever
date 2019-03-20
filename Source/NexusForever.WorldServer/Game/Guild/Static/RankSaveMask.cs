using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Plot"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum RankSaveMask
    {
        None        = 0x0000,
        Create      = 0x0001,
        Delete      = 0x0002,
        Rename      = 0x0004,
        Permissions = 0x0008,
        Index       = 0x00016,
    }
}
