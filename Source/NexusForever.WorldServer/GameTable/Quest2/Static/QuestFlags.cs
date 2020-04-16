using System;

namespace NexusForever.WorldServer.GameTable.Quest2.Static
{
    [Flags]
    public enum QuestFlags
    {
        None         = 0x0000,
        AutoComplete = 0x0001,
        Optional     = 0x0008,
    }
}
