namespace NexusForever.Game.Static.ICComm
{
    public enum ICCommJoinResult
    {
        TooManyChannels    = 0x1,
        NoGuild            = 0x2,
        NoGroup            = 0x3,
        BadName            = 0x4,
        Join               = 0x5,
        Left               = 0x6,
        MissingEntitlement = 0x7
    };
}
