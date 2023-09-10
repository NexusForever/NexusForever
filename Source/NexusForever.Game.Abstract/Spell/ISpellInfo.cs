using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell
{
    public interface ISpellInfo
    {
        Spell4Entry Entry { get; }
        ISpellBaseInfo BaseInfo { get; }
        Spell4AoeTargetConstraintsEntry AoeTargetConstraints { get; }
        Spell4ConditionsEntry CasterConditions { get; }
        Spell4ConditionsEntry TargetConditions { get; }
        Spell4CCConditionsEntry CasterCCConditions { get; }
        Spell4CCConditionsEntry TargetCCConditions { get; }
        SpellCoolDownEntry GlobalCooldown { get; }
        Spell4StackGroupEntry StackGroup { get; }
        PrerequisiteEntry CasterCastPrerequisite { get; }
        PrerequisiteEntry TargetCastPrerequisites { get; }
        PrerequisiteEntry CasterPersistencePrerequisites { get; }
        PrerequisiteEntry TargetPersistencePrerequisites { get; }
        List<PrerequisiteEntry> PrerequisiteRunners { get; }
        Spell4VisualGroupEntry VisualGroup { get; }

        List<TelegraphDamageEntry> Telegraphs { get; }
        List<Spell4EffectsEntry> Effects { get; }

        List<Spell4VisualEntry> Visuals { get; }
    }
}