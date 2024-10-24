using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;

namespace NexusForever.Game.Abstract
{
    public interface IDamageCalculator
    {
        void CalculateDamage(IUnitEntity attacker, IUnitEntity victim, ISpell spell, ISpellTargetEffectInfo info);
    }
}
