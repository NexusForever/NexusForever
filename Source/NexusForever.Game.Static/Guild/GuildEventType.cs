namespace NexusForever.Game.Static.Guild
{
    public enum GuildEventType
    {
        Achievement       = 0,
        PerkUnlock        = 1,
        PerkActivate      = 2,
        MemberAdded       = 3,
        MemberRemoved     = 4,
        MemberRankChanged = 5,
        MessageOfTheDay   = 6,
        BankMoneyDeposit  = 1000, // Not sure which is deposit and withdraw
        BankMoneyWithdraw = 1001,
        ItemRepair        = 2000,
        BankTab           = 100000,
    };
}
