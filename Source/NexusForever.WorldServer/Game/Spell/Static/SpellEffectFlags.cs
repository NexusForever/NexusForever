using System;

namespace NexusForever.WorldServer.Game.Spell.Static
{
    [Flags]
    public enum SpellEffectFlags
    {
        None       = 0x00,
        CancelOnly = 0x02,
    }
}
