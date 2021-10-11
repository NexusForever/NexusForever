using System;

namespace NexusForever.WorldServer.Game.Housing.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Residence"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum ResidenceSaveMask
    {
        None            = 0x000000,
        Create          = 0x000001,
        Name            = 0x000002,
        Wallpaper       = 0x000004,
        Roof            = 0x000008,
        Entryway        = 0x000010,
        Door            = 0x000020,
        Ground          = 0x000040,
        Sky             = 0x000080,
        Flags           = 0x000100,
        ResourceSharing = 0x000200,
        GardenSharing   = 0x000400,
        Decor           = 0x000800,
        Plot            = 0x001000,
        PrivacyLevel    = 0x002000,
        Music           = 0x004000,
        PropertyInfo    = 0x008000,
        GuildOwner      = 0x010000
    }
}
