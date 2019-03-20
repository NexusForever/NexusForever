using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Plot"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum GuildBaseSaveMask
    {
        None        = 0x0000,
        Create      = 0x0001,
        Delete      = 0x0002,
    }
}
