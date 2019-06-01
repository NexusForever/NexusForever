using System;

namespace NexusForever.MapGenerator
{
    [Flags]
    public enum Flags
    {
        None                   = 0x00,
        Extraction             = 0x01,
        Generation             = 0x02,
        GenerationSingleThread = 0x04
    }
}
