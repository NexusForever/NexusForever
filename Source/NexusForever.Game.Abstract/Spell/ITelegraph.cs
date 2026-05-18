using System.Numerics;
using NexusForever.Game.Abstract.Entity;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ITelegraph
    {
        IUnitEntity Caster { get; }
        Vector3 Position { get; }
        Vector3 Rotation { get; }
        TelegraphDamageEntry TelegraphDamage { get; }

        /// <summary>
        /// Returns any <see cref="IUnitEntity"/> inside the <see cref="ITelegraph"/>.
        /// </summary>
        IEnumerable<IUnitEntity> GetTargets();

        /// <summary>
        /// Returns whether the supplied <see cref="Vector3"/> is inside the telegraph.
        /// </summary>
        bool InsideTelegraph(Vector3 position, float hitRadius);
    }
}