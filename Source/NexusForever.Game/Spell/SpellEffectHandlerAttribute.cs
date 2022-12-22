using NexusForever.Game.Static.Spell;

namespace NexusForever.Game.Spell
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
