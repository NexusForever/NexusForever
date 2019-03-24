using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum ShortcutSaveMask
    {
        None         = 0x0000,
        Create       = 0x0001,
        Delete       = 0x0002,
        ObjectId     = 0x0004,
        Tier         = 0x0008,
        ShortcutType = 0x0010
    }
}
