using System;

namespace NexusForever.Game.Static.Group
{
    [Flags]
    public enum GroupFlags
    {
        OpenWorld               = 1 << 0,
        Raid                    = 1 << 1,
        JoinRequestOpen         = 1 << 4,
        JoinRequestClosed       = 1 << 5,
        ReferralsOpen           = 1 << 6,
        ReferralsClosed         = 1 << 7
    }
}
