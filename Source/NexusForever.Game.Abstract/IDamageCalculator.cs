using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Spell;
using System.ComponentModel;

namespace NexusForever.Game.Abstract
{
    public interface IDamageCalculator
    {
        void Initialise();
        void CalculateDamage(IUnitEntity attacker, IUnitEntity victim, ISpell spell, ref ISpellTargetEffectInfo info, DamageType damageType, uint damage);
        uint GetBaseDamageForSpell(IUnitEntity attacker, float parameterType, float parameterValue);
    }
}