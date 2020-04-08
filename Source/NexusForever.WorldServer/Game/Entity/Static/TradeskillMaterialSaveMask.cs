using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="TradeskillMaterial"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum TradeskillMaterialSaveMask
    {
        None                = 0x0000,
        Create              = 0x0001,
        Amount               = 0x0002
    }
}
