namespace NexusForever.GameTable.Static
{
    [Flags]
    public enum ItemFlags
    {
        None            = 0x00000000,
        DestroyOnLogout = 0x00000040,
        DestroyOnZone   = 0x00000080,
        Unknown200      = 0x00000200, // seems to relate to items that sell for WarCoins
        Depreciated     = 0x00004000,
        PlayerVsPlayer  = 0x00008000
    }
}
