using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="GuildBase"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum GuildBaseSaveMask
    {
        None     = 0x0000,
        Create   = 0x0001,
        Delete   = 0x0002,
        Name     = 0x0004,
        LeaderId = 0x0008,
        Flags    = 0x0010
    }
}
