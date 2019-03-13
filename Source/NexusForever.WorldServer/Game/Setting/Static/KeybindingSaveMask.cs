using System;

namespace NexusForever.WorldServer.Game.Setting.Static
{
    [Flags]
    public enum KeybindingSaveMask
    {
        None   = 0x00,
        Create = 0x01,
        Modify = 0x02,
        Delete = 0x04
    }
}
