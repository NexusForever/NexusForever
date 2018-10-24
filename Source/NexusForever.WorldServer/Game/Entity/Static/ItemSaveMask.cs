using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Item"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum ItemSaveMask
    {
        None               = 0x0000,
        Create             = 0x0001,
        Delete             = 0x0002,
        CharacterId        = 0x0004,
        Location           = 0x0008,
        BagIndex           = 0x0010,
        StackCount         = 0x0020,
        Charges            = 0x0040,
        Durability         = 0x0080,
        ExpirationTimeLeft = 0x0100
    }
}
