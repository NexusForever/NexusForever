namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum CostumeSaveResult
    {
        Saved                   = 0,
        InvalidCostumeIndex     = 1,
        CostumeIndexNotUnlocked = 2,
        InvalidMannequinIndex   = 3,
        UnknownMannequinError   = 4,
        ItemNotUnlocked         = 5,
        InvalidItem             = 6,
        UnusableItem            = 7,
        InvalidDye              = 8,
        DyeNotUnlocked          = 9,
        NotEnoughTokens         = 10,
        InsufficientFunds       = 11,
        UnknownError            = 12,
        Saving                  = 13
    }
}
