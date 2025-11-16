namespace NexusForever.Game.Static.ICComm
{
    public enum ICCommMessageResult
    {
        Sent               = 0x1,
        Throttled          = 0x2,
        NotInChannel       = 0x3,
        InvalidText        = 0x7,
        MissingEntitlement = 0x8
    };
}
