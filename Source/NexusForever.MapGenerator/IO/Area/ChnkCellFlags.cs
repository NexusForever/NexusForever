using System;

namespace NexusForever.MapGenerator.IO.Area
{
    [Flags]
    public enum ChnkCellFlags
    {
        HeightMap = 0x00000001,
        ZoneBound = 0x00010000,
        Zone      = 0x10000000
    }
}
