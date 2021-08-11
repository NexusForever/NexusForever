using System;

namespace NexusForever.WorldServer.Game.Housing.Static
{
    /// <summary>
    /// Determines which fields need saving for <see cref="Plot"/> when being saved to the database.
    /// </summary>
    [Flags]
    public enum PlotSaveMask
    {
        None       = 0x0000,
        Create     = 0x0001,
        PlugItemId = 0x0002,
        PlugFacing = 0x0004,
        BuildState = 0x0008,
        PlotInfoId = 0x0010
    }
}
