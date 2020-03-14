using System;

namespace NexusForever.WorldServer.Game.Quest.Static
{
    [Flags]
    public enum QuestSaveMask
    {
        None   = 0x00,
        Create = 0x01,
        State  = 0x02,
        Flags  = 0x04,
        Reset  = 0x08,
        Delete = 0x10,
        Timer  = 0x20
    }
}
