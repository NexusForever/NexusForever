using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.CSI.Static
{
    public enum CSIType
    {
        Interaction         = -1,
        PressAndHold        = 0,
        RapidTapping        = 1,
        PrecisionTapping    = 2,
        Metronome           = 3,
        YesNo               = 4,
        Memory              = 5,
        Keypad              = 6,
        RapidTappingInverse = 7,
    }
}
