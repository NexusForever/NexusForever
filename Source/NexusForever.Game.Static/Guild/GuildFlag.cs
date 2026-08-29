namespace NexusForever.Game.Static.Guild
{
    [Flags]
    public enum GuildFlag
    {
        None             = 0x00,
        Taxes            = 0x01,
        Recruiting       = 0x02,
        Mercenary        = 0x04,
        Rename           = 0x08,
        CommunityPrivate = 0x10,
    }
}
