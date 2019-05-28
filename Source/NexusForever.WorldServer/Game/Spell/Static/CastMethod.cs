using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Spell.Static
{
    public enum CastMethod
    {
        Normal                  = 0,
        Channeled               = 1,
        PressHold               = 2,
        ChanneledField          = 3,
        UNUSED04                = 4,
        ClientSideInteraction   = 5,
        RapidTap                = 6,
        ChargeRelease           = 7,
        Multiphase              = 8,
        Transactional           = 9,
        Aura                    = 10
    }
}
