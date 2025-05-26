namespace NexusForever.Game.Static.Challenges
{
    public enum ChallengeResult
    {
        Unlock              = 0x0,
        Activate            = 0x1,
        AbandonRemoveArrows = 0x3,
        TimerExpired        = 0x4,
        LeftArea            = 0x5,
        GenericFail         = 0x6, // Uses Data as localizedStringId if set
        AreaRestriction     = 0x7,
        Completed           = 0x8, // Uses Data as tier value, 0 indexed
        TierAchieved        = 0x9, // Uses Data as tier value, 0 indexed
        TypeAlreadyActive   = 0xA,
        CooldownActive      = 0xB,
        LeftAreaWarning     = 0xC,
        ResetAll            = 0xD,
        AbandonRemove       = 0x10,
        JoinedRaidFail      = 0x11
    }
}
