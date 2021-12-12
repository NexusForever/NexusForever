using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ITelegraph
    {
        IUnitEntity Caster { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; }
        TelegraphDamageEntry TelegraphDamage { get; }
        TelegraphTargetTypeFlags TelegraphTargetTypeFlags { get; }

        /// <summary>
        /// Returns a <see cref="IEnumerable{T}"/> containing all <see cref="ISpellTargetInfo"/> that can be targeted by this <see cref="ITelegraph"/>.
        /// </summary>
        IEnumerable<ISpellTargetInfo> GetTargets(ISpell spell, List<ISpellTargetInfo> targets);

        /// <summary>
        /// Returns whether the supplied <see cref="Vector3"/> is inside the telegraph.
        /// </summary>
        bool InsideTelegraph(Vector3 position, float hitRadius);
    }
}