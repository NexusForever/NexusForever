using System;

namespace NexusForever.WorldServer.Game.Housing.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Decor"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum DecorSaveMask
    {
        None               = 0x0000,
        Create             = 0x0001,
        Delete             = 0x0002,
        Type               = 0x0004,
        Position           = 0x0008,
        Rotation           = 0x0010,
        Scale              = 0x0020,
        DecorParentId      = 0x0040,
        ColourShiftId      = 0x0080,
        PlotIndex          = 0x0100
    }
}
