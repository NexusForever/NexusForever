using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Guild"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum GuildSaveMask
    {
        None            = 0x0000,
        MessageOfTheDay = 0x0001,
        AdditionalInfo  = 0x0002
    }
}
