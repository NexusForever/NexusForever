using NexusForever.Game.Abstract.Entity;

namespace NexusForever.Game.Abstract.Spell
{
    public delegate void SpellEffectDelegate(ISpell spell, IUnitEntity target, ISpellTargetEffectInfo info);
}