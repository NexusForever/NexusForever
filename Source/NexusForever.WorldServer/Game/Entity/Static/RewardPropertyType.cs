namespace NexusForever.WorldServer.Game.Entity.Static
{
    /// <summary>
    /// Replication of the CodeEnumRewardProperty enum from the client. Gaps for missing numbers.
    /// </summary>
    public enum RewardPropertyType
    {
        XP                          = 1,
        CurrencyType                = 2,
        RewardTrackId               = 3,
        RewardTrackType             = 4,
        OmniBits                    = 5,

        Reputation                  = 7,
        OmnibitDropChance           = 8,
        RestXpAccrual               = 9,
        RestXpCap                   = 10,
        CostumeSlots                = 11,
        TradeskillMatTrading        = 12,
        BankSlots                   = 13,
        Trading                     = 14,
        CommodityOrders             = 15,
        AuctionBids                 = 16,
        AuctionListings             = 17,
        AuctionAccess               = 18,
        CommodityAccess             = 19,
        MoneyTradeLimit             = 20,
        GuildCreateOrInviteAccess   = 21,

        CircleMembershipUnlimited   = 23,

        TradeskillMatStackLimit     = 25,
        BagSlots                    = 26,
        WakeHereCooldown            = 27,
        GuildHolomarkUnlimited      = 28,
        CharacterSlots              = 29,
        PurchaseDiscount            = 30,
        ExtraDecorSlots             = 31,

        OmnibitCap                  = 33,
        WorldPvpPrestigeBonus       = 34,
        WorldPvpPrestigeCap         = 35,

        ActiveContractSlots         = 37,
        RotationEssences            = 38,
        AccountCurrency             = 39
    }
}
