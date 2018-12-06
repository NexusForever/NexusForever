using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Currency"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum CurrencySaveMask
    {
        None               = 0x0000,
        Create             = 0x0001,
        Delete             = 0x0002,
        Amount             = 0x0004,
    }
}
