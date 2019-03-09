using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    [Flags]
    public enum CostumeUnlockSaveMask
    {
        None   = 0x00,
        Create = 0x01,
        Delete = 0x02
    }
}
