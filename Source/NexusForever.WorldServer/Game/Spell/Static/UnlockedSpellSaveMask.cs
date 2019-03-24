using System;

namespace NexusForever.WorldServer.Game.Spell.Static
{
    [Flags]
    public enum UnlockedSpellSaveMask
    {
        None   = 0x0000,
        Create = 0x0001,
        Tier   = 0x0002
    }
}
