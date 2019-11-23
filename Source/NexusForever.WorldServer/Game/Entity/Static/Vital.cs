using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum Vital
    {
        Invalid          = 0x0000,
        Health           = 0x0001,
        Breath           = 0x0002,
        ShieldCapacity   = 0x0003,
        KineticEnergy    = 0x0004,
        Endurance        = 0x0005,
        Resource1        = 0x0006,
        Resource2        = 0x0007,
        Resource3        = 0x0008,
        Resource4        = 0x0009,
        Resource5        = 0x000A,
        Resource6        = 0x000B,
        SuitPower        = 0x000C,
        Actuator         = 0x000D,
        StalkerC         = 0x000E,
        Focus            = 0x000F,
        Dash             = 0x0010,
        // X 17
        Actuator2        = 0x0012,
        SpellSurge       = 0x0013,
        InterruptArmor   = 0x0014,
        Absorption       = 0x0015,
        PublicResource0  = 0x0016,
        PublicResource1  = 0x0017,
        PublicResource2  = 0x0018,
        // X 25
        Volatility       = 0x001A,
        Resource8        = 0x001B,
        Resource9        = 0x001C,
        Resource10       = 0x001D,
        HealingAbsorption = 0x001E
    }
}
