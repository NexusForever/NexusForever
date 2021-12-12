using NexusForever.Game.Static.Spell;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpellBaseInfo : IEnumerable<ISpellInfo>
    {
        Spell4BaseEntry Entry { get; }
        Spell4HitResultsEntry HitResult { get; }
        Spell4TargetMechanicsEntry TargetMechanics { get; }
        Spell4TargetAngleEntry TargetAngle { get; }
        Spell4PrerequisitesEntry Prerequisites { get; }
        Spell4ValidTargetsEntry ValidTargets { get; }
        TargetGroupEntry CastGroup { get; }
        Creature2Entry PositionalAoe { get; }
        TargetGroupEntry AoeGroup { get; }
        Spell4BaseEntry PrerequisiteSpell { get; }
        Spell4SpellTypesEntry SpellType { get; }
        SpellClass SpellClass { get; }
        bool HasIcon { get; }
        bool IsDebuff { get; }
        bool IsBuff { get; }
        bool IsDispellable { get; }

        /// <summary>
        /// Return <see cref="ISpellInfo"/> for the supplied spell tier.
        /// </summary>
        ISpellInfo GetSpellInfo(byte tier);
    }
}