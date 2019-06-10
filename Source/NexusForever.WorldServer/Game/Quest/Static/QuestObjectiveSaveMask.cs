using System;

namespace NexusForever.WorldServer.Game.Quest.Static
{
    [Flags]
    public enum QuestObjectiveSaveMask
    {
        None     = 0x00,
        Create   = 0x01,
        Progress = 0x02,
        Timer    = 0x04
    }
}
