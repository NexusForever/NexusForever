using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    [Flags]
    public enum GuildFlag
    {
        None             = 0x00,
        Taxes            = 0x01,
        CommunityPrivate = 0x10
    }
}
