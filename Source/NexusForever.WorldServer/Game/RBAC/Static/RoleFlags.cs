using System;

namespace NexusForever.WorldServer.Game.RBAC.Static
{
    [Flags]
    public enum RoleFlags
    {
        None      = 0x00,
        Inclusive = 0x01,
        Exclusive = 0x02
    }
}
