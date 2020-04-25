using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Account.Static
{
    public enum AccountOperationResult
    {
        Ok                        = 0x0000,
        GenericFail               = 0x0001,
        DBError                   = 0x0002,
        MTXError                  = 0x0003,
        InvalidOffer              = 0x0004,
        InvalidPrice              = 0x0005,
        NotEnoughCurrency         = 0x0006,
        NeedTransaction           = 0x0007,
        InvalidAccountItem        = 0x0008,
        InvalidPendingItem        = 0x0009,
        InvalidInventoryItem      = 0x000A,
        NoConnection              = 0x000B,
        NoCharacter               = 0x000C,
        AlreadyClaimed            = 0x000D,
        MaxEntitlementCount       = 0x000E,
        NoRegift                  = 0x000F,
        NoGifting                 = 0x0010,
        InvalidFriend             = 0x0011,
        InvalidCoupon             = 0x0012,
        CannotReturn              = 0x0013,
        Prereq                    = 0x0014,
        CREDDExchangeNotLoaded    = 0x0015,
        NoCREDD                   = 0x0016,
        NoMatchingOrder           = 0x0017,
        InvalidCREDDOrder         = 0x0018,
        Cooldown                  = 0x0019,
        MissingEntitlement        = 0x001A,
        AlreadyClaimedMultiRedeem = 0x001B,
        PremiumOnly               = 0x001C
    }
}
