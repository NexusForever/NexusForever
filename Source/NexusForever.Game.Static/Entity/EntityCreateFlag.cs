namespace NexusForever.Game.Static.Entity
{
    [Flags]
    public enum EntityCreateFlag
    {
        None                         = 0x00,
        UseDefaultBirthSequence      = 0x01,
        DontTimeAdjustInitialMovementCommands = 0x02,
        IsVendor                     = 0x04,
        IsStealthed                  = 0x10,
        MinimapMarkerHidden          = 0x20,
        IsHoveringUnit               = 0x40,
        NoDeathDelay                 = 0x80 // death model sequence starts with no delay
    }
}
