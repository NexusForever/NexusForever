using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="SpellManager"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum SpellManagerSaveMask
    {
        None            = 0x0000,
        ActiveActionSet = 0x0001,
        Innate          = 0x0002,
    }
}
