using System;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    // TODO: research more, see WorldEntity::CreateFromPacket in IDB
    [Flags]
    public enum EntityCreateFlag
    {
        None             = 0x00,
        SpawnAnimation   = 0x01,
        NoSpawnAnimation = 0x02, // ??
        Unknown02        = 0x02,
        Vendor           = 0x04,
        // 0x08 ??
        Unknown10        = 0x10,
        Unknown20        = 0x20,
        Unknown40        = 0x40
    }
}
