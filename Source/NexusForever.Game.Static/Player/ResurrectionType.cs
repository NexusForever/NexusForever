namespace NexusForever.Game.Static.Player
{
    [Flags]
    public enum ResurrectionType
    {
        None                    = 0x00,
        WakeHere                = 0x01,
        Holocrypt               = 0x02,
        SpellCasterLocation     = 0x04,
        ExitInstance            = 0x20,
        WakeHereServiceToken    = 0x40,

        // TODO: Add Holocrypt to below masks when we support them
        OpenWorld               = WakeHere | WakeHereServiceToken,
        Dungeon                 = ExitInstance
    }
}
