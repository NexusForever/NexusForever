using System;

namespace NexusForever.WorldServer.Game.Housing.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Residence"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum ResidenceSaveMask
    {
        None            = 0x0000,
        Create          = 0x0001,
        Name            = 0x0002,
        Wallpaper       = 0x0004,
        Roof            = 0x0008,
        Entryway        = 0x0010,
        Door            = 0x0020,
        Ground          = 0x0040,
        Sky             = 0x0080,
        Flags           = 0x0100,
        ResourceSharing = 0x0200,
        GardenSharing   = 0x0400,
        Decor           = 0x0800,
        Plot            = 0x1000,
        PrivacyLevel    = 0x2000,
        Music           = 0x4000,
        ResidenceInfo   = 0x8000
    }
}
