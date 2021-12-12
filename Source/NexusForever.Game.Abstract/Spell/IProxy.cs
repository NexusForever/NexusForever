using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Abstract.Spell.Event;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface IProxy
    {
        bool CanCast { get; }
        Spell4EffectsEntry Entry { get; }
        ISpell ParentSpell { get; }
        IUnitEntity Target { get; }

        void Cast(IUnitEntity caster, ISpellEventManager events);
        void Evaluate();
    }
}