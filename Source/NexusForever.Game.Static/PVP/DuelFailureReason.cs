namespace NexusForever.Game.Static.Pvp
{
    public enum DuelFailureReason
    {
        YouHaveDuelRequestPending         = 1,
        PlayerHasDuelRequestPending       = 2,
        YouAreInCombat                    = 3,
        PlayerIsInCombat                  = 4,
        YouAreAlreadyDueling              = 5,
        PlayerIsAlreadyDueling            = 6,
        CannotDuelRightNow                = 7,
        TooFarAway                        = 8,
        CanOnlyDuelSameFaction            = 9,
        PlayerInAnotherPhase              = 10,
        PlayerIsIgnoringYou               = 11,
        InvalidDuelTarget                 = 12,
        CannotDuelHere                    = 13,
        PlayerCannotDuelAtThatLocation    = 14,
        PlayerIsIgnoringDuels             = 15,
        YouCannotDuelWhileDead            = 16,
        CannotDuelDeadPlayer              = 17,
        CannotDuelInCrossFactionGroup     = 18,
    }
}
