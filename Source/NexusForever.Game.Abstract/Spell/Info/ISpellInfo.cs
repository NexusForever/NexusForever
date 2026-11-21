using NexusForever.Game.Spell.Info;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Abstract.Spell.Info
{
    public interface ISpellInfo
    {
        Spell4Entry Entry { get; }
        ISpellBaseInfo BaseInfo { get; set; }
        Spell4AoeTargetConstraintsEntry AoeTargetConstraints { get; }
        Spell4ConditionsEntry CasterConditions { get; }
        Spell4ConditionsEntry TargetConditions { get; }
        Spell4CCConditionsEntry CasterCCConditions { get; }
        Spell4CCConditionsEntry TargetCCConditions { get; }
        SpellCoolDownEntry GlobalCooldown { get; }
        Spell4StackGroupEntry StackGroup { get; }
        PrerequisiteEntry CasterCastPrerequisite { get; }
        PrerequisiteEntry TargetCastPrerequisite { get; }
        PrerequisiteEntry CasterPersistencePrerequisite { get; }
        PrerequisiteEntry TargetPersistencePrerequisite { get; }
        List<PrerequisiteEntry> PrerequisiteRunners { get; }

        List<TelegraphDamageEntry> Telegraphs { get; }
        List<Spell4EffectsEntry> Effects { get; }
        List<Spell4ThresholdsEntry> Thresholds { get; }
        List<SpellPhaseEntry> Phases { get; }

        Spell4VisualGroupEntry VisualGroup { get; }
        List<Spell4VisualEntry> Visuals { get; }
        List<SpellCoolDownEntry> Cooldowns { get; }

        public HashSet<uint> SpellGroups { get; }

        /// <summary>
        /// Initialise the <see cref="ISpellInfo"/> with the supplied <see cref="Spell4Entry"/> and <see cref="ISpellInfoCache"/>.
        /// </summary>
        void Initialise(Spell4Entry spell4Entry, ISpellInfoCache spellInfoCache);

        /// <summary>
        /// Perform any late initialisation tasks that need to complete after all <see cref="ISpellInfo"/> have been initialised.
        /// </summary>
        void InitialiseLate();

        /// <summary>
        /// Return <see cref="ISpellInfo"/> for a given Threshold Index.
        /// </summary>
        (ISpellInfo, Spell4ThresholdsEntry) GetThresholdSpellInfo(int index);
    }
}
