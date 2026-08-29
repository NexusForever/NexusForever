namespace NexusForever.Game.Static.Group
{
    public enum GroupActionResult
    {
        LeaveSuccess           = 1,
        LeaveFailed            = 2,
        DisbandSuccess         = 3,
        DisbandFailed          = 4,
        KickSuccess            = 5,
        KickFailed             = 6,
        PromoteSuccess         = 7,
        PromoteFailed          = 8,
        FlagsSuccess           = 9,
        FlagsFailed            = 10,
        MemberFlagsSuccess     = 11,
        MemberFlagsFailed      = 12,
        NotInGroup             = 13,
        ChangeSettingsSuccess  = 14,
        ChangeSettingsFailed   = 15,
        MentoringInvalidMentor = 16,
        MentoringInvalidMentee = 17,
        InvalidGroup           = 18,
        MentoringSelf          = 19,
        ReadyCheckFailed       = 20,
        MarkingNotPermitted    = 21,
        InvalidMarkIndex       = 22,
        MentoringNotAllowed    = 23,
        InvalidMarkTarget      = 24,
        MentoringInCombat      = 25,
        MentoringLowestLevel   = 26,
        AlreadyInGroupInstance = 27,
        MentoringNoAction      = 29,
        OrderInvalidMember     = 30,
        OrderFailedLeader      = 31,
        OrderFailedInUse       = 33
    }
}
