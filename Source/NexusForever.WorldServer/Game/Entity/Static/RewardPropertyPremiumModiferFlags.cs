using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum RewardPropertyPremiumModiferFlags
    {
        None        = 0x0,
        // this determines if a modifier entry falls through to the next tier
        // if this is set for tier 0 it will be included for tier 1 as well
        FallThrough = 0x1
    }
}
