namespace NexusForever.Game.Static.Crafting
{
    public enum TradeskillResult
    {
        Success                 = 0,
        InsufficentFund         = 1,
        InvalidItem             = 2,
        InvalidSlot             = 3,
        MissingEngravingStation = 4,
        Unlocked                = 5,
        UnknownError            = 6,
        RuneExists              = 7,
        MissingRune             = 8,
        DuplicateRune           = 9,
        AttemptFailed           = 10,
        RuneSlotLimit           = 11,
    };
}
