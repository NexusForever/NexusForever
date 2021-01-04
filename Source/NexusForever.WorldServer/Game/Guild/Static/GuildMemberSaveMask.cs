using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="GuildMember"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum GuildMemberSaveMask
    {
        None   = 0x0000,
        Create = 0x0001,
        Delete = 0x0002,
        Rank   = 0x0004,
        Note   = 0x0008
    }
}
