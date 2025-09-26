namespace NexusForever.Game.Static.AccountInventory
{
    public enum AccountOperation
    {
        MTXPurchase = 0x0,
        ClaimPending = 0x1,
        ReturnPending = 0x2,
        TakeItem = 0x3,
        GiftItem = 0x4,
        RedeemCoupon = 0x5,
        GetCREDDExchangeInfo = 0x6,
        SellCREDD = 0x7,
        BuyCREDD = 0x8,
        CancelCREDDOrder = 0x9,
        ExpireCREDDOrder = 0xA,
        SellCREDDComplete = 0xB,
        BuyCREDDComplete = 0xC,
        CREDDRedeem = 0xF,
        RequestDailyLoginRewards = 0x10,
        RequestPremiumLockboxKey = 0x11,
    };
}
