using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Spell.Static
{
    public enum CastMethod
    {
        Normal                = 0x0000,
        Channeled             = 0x0001,
        PressHold             = 0x0002,
        ChanneledField        = 0x0003,
        UNUSED04              = 0x0004,
        ClientSideInteraction = 0x0005,
        RapidTap              = 0x0006,
        ChargeRelease         = 0x0007,
        Multiphase            = 0x0008,
        Transactional         = 0x0009,
        Aura                  = 0x000A
    }
}