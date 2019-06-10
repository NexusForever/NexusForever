using System;

namespace NexusForever.WorldServer.Game.Quest.Static
{
    [Flags]
    public enum QuestFlags
    {
        None    = 0x00,
        Tracked = 0x02
    }
}
