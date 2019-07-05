using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.GameTable.Quest2.Static
{
    [Flags]
    public enum QuestFlags
    {
        None            = 0x0000,
        NoObjectives    = 0x0001,
        Optional        = 0x0008,
    }
}
