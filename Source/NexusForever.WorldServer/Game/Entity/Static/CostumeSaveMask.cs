using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Costume"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum CostumeSaveMask
    {
        None   = 0x00,
        Create = 0x01,
        Mask   = 0x02
    }
}
