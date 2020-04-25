using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Account.Static
{
    public enum AccountOperation
    {
        MTXPurchase              = 0x0000,
        ClaimPending             = 0x0001,
        ReturnPending            = 0x0002,
        TakeItem                 = 0x0003,
        GiftItem                 = 0x0004,
        RedeemCoupon             = 0x0005,
        GetCREDDExchangeInfo     = 0x0006,
        SellCREDD                = 0x0007,
        BuyCREDD                 = 0x0008,
        CancelCREDDOrder         = 0x0009,
        ExpireCREDDOrder         = 0x000A,
        SellCREDDComplete        = 0x000B,
        BuyCREDDComplete         = 0x000C,
        CREDDRedeem              = 0x000F,
        RequestDailyLoginRewards = 0x0010,
        RequestPremiumLockboxKey = 0x0011
    }
}
