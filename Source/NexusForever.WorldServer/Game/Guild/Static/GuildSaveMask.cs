using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Plot"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum GuildSaveMask
    {
        None            = 0x0000,
        Create          = 0x0001,
        MessageOfTheDay = 0x0002,
        AdditionalInfo  = 0x0004,
        Taxes           = 0x0008,
    }
}
