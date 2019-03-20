using System;

namespace NexusForever.WorldServer.Game.Guild.Static
{
    [Flags]
    public enum GuildHolomark
    {
        None        = 0x0000,
        Back        = 0x0001,
        Left        = 0x0002,
        Right       = 0x0004,
        Near        = 0x0008
    }
}
