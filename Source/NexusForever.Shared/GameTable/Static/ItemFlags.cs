using System;

namespace NexusForever.Shared.GameTable.Static;

[Flags]
public enum ItemFlags
{
    None            = 0x00000000,
    DestroyOnLogout = 0x00000040,
    DestroyOnZone   = 0x00000080,
    Depreciated     = 0x00004000,
    PlayerVsPlayer  = 0x00008000
}
