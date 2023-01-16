﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Spell.Static
{
    /// <summary>
    /// TelegraphDamageFlags in the TelegraphEntry TBL seem to all be calculated server side. Unable to find information in the client on them.
    /// This is assumed value meaning based on complete speculation when comparing telegraphs.
    /// </summary>
    [Flags]
    public enum TelegraphDamageFlag
    {
        SpellMustBeMultiPhase   = 0x0020,
        TargetMustBeUnit        = 0x0200,
        CasterMustBePlayer      = 0x0400,
        CasterMustBeNPC         = 0x1000,
    }
}
