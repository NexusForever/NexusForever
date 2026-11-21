namespace NexusForever.Game.Static.Entity
{
    [Flags]
    public enum EntityCreateFlag
    {
        None                    = 0x00,
        UseDefaultBirthSequence = 0x01,
        Immediate               = 0x02, // does not time adjust initial movement commands
        HasInteractionPrereq    = 0x04, // mostly used for vendors but also seemingly for cinematic actor units
        Unknown08               = 0x08,
        IsStealthed             = 0x10,
        MinimapMarkerHidden     = 0x20,
        IsHoveringUnit          = 0x40,
        NoDeathDelay            = 0x80 // death model sequence starts with no delay
    }
}
