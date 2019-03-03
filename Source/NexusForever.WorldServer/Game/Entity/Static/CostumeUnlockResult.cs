namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum CostumeUnlockResult
    {
        UnlockSuccess       = 0,
        AlreadyKnown        = 1,
        OutOfSpace          = 2,
        UnknownFailure      = 3,
        ForgetItemSuccess   = 4,
        ForgetItemFailed    = 5,
        FailedPrerequisites = 6,
        InsufficientCredits = 7,
        ItemInUse           = 8,
        ItemNotKnown        = 9,
        UnlockRequested     = 10,
        ForgetRequested     = 11,
        InvalidItem         = 12
    }
}
