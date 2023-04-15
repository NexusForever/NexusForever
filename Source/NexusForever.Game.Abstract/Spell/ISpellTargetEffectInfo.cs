using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpellTargetEffectInfo
    {
        uint EffectId { get; }
        Spell4EffectsEntry Entry { get; }
        IDamageDescription Damage { get; }

        void AddDamage(DamageType damageType, uint damage);
    }
}