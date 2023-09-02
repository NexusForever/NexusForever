using System;

namespace NexusForever.Game.Static.Group
{
    [Flags]
    public enum GroupMemberInfoFlags
    {
        CanInvite           = 1 << 1,
        CanKick             = 1 << 2,
        Disconnected        = 1 << 3,
        Pending             = 1 << 4,
        Tank                = 1 << 5,
        Healer              = 1 << 6,
        DPS                 = 1 << 7,
        MainTank            = 1 << 8,
        MainAssist          = 1 << 9,
        RaidAssistant       = 1 << 10,
        Ready               = 1 << 11,
        RoleLocked          = 1 << 12,
        CanMark             = 1 << 13,
        HasSetReady         = 1 << 14,

        GroupMemberFlags    = CanMark,
        GroupAdminFlags     = CanInvite | CanKick | CanMark,
        GroupAssistantFlags = RaidAssistant | GroupAdminFlags,

        MainAssistFlags     = MainAssist | CanMark,
        MainTankFlags       = MainTank | CanMark,

        CanReadyCheck       = RaidAssistant | MainAssist | MainTank,

        RoleFlags           = Tank | Healer | DPS
    }
}
