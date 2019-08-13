using System;

namespace NexusForever.MapGenerator.IO.Area
{
    [Flags]
    public enum ChnkCellFlags
    {
        HeightMap = 0x00000001,
        Area      = 0x10000000
    }
}
