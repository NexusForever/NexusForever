using System;
using NexusForever.WorldServer.Game.Spell.Static;

namespace NexusForever.WorldServer.Game.Spell
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SpellEffectHandlerAttribute : Attribute
    {
        public SpellEffectType SpellEffectType { get; }

        public SpellEffectHandlerAttribute(SpellEffectType spellEffectType)
        {
            SpellEffectType = spellEffectType;
        }
    }
}
