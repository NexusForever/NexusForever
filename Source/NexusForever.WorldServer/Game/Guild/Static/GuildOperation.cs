namespace NexusForever.WorldServer.Game.Guild.Static
{
    public enum GuildOperation
    {
        Disband                 = 1,
        RankAdd                 = 2,
        RankDelete              = 3,
        RankPermissions         = 4,
        MemberPromote           = 6,
        MemberDemote            = 7,
        MemberInvite            = 8,
        MemberRemove            = 9,
        MemberQuit              = 10,
        RosterRequest           = 11,
        RankRename              = 15,
        SetNameplateAffiliation = 16,
        MakeLeader              = 21,
        BankTab1Change          = 23,
        WithdrawalLimitChange   = 24,
        UnlockTier              = 25,
        MessageOfTheDay         = 27,
        EditPlayerNote          = 28,
        TaxUpdate               = 29,
        RepairLimitChange       = 30,
        InitGuildWindow         = 32,
        AdditionalInfo          = 33
    }
}
