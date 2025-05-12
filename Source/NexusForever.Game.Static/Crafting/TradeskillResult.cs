namespace NexusForever.Game.Static.Crafting
{
    public enum TradeskillResult
    {
        Success                  = 0x0,
        InsufficentFund          = 0x1,
        InvalidItem              = 0x2,
        InvalidSlot              = 0x3,
        MissingEngravingStation  = 0x4,
        Unlocked                 = 0x5,
        UnknownError             = 0x6,
        RuneExists               = 0x7,
        MissingRune              = 0x8,
        DuplicateRune            = 0x9,
        AttemptFailed            = 0xA,
        RuneSlotLimit            = 0xB,
    };
}
