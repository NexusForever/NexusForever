using System;

namespace NexusForever.Game.Static.Contact
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Contact"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum ContactSaveMask
    {
        None   = 0x00,
        Create = 0x01,
        Delete = 0x02,
        Modify = 0x04
    }
}
