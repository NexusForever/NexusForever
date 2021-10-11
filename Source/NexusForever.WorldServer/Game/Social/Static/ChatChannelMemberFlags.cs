using System;

namespace NexusForever.WorldServer.Game.Social.Static
{
    [Flags]
    public enum ChatChannelMemberFlags
    {
        None      = 0x00,
        Owner     = 0x01,
        Moderator = 0x02,
        Muted     = 0x04
    }
}
