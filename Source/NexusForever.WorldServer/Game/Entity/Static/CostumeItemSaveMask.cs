using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="CostumeItem"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum CostumeItemSaveMask
    {
        None    = 0x00,
        Create  = 0x01,
        ItemId  = 0x02,
        DyeData = 0x04
    }
}
