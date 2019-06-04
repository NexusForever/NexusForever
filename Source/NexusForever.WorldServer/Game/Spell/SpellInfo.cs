using System.Collections.Generic;
using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game.Spell
{
    public class SpellInfo
    {
        public Spell4Entry Entry { get; }
        public SpellBaseInfo BaseInfo { get; }
        public Spell4AoeTargetConstraintsEntry AoeTargetConstraints { get; }
        public Spell4ConditionsEntry CasterConditions { get; }
        public Spell4ConditionsEntry TargetConditions { get; }
        public Spell4CCConditionsEntry CasterCCConditions { get; }
        public Spell4CCConditionsEntry TargetCCConditions { get; }
        public SpellCoolDownEntry GlobalCooldown { get; }
        public Spell4StackGroupEntry StackGroup { get; }
        public PrerequisiteEntry CasterCastPrerequisites { get; }
        public PrerequisiteEntry TargetCastPrerequisites { get; }
        public PrerequisiteEntry CasterPersistencePrerequisites { get; }
        public PrerequisiteEntry TargetPersistencePrerequisites { get; }

        public List<TelegraphDamageEntry> Telegraphs { get; }
        public List<Spell4EffectsEntry> Effects { get; }
        public List<Spell4ThresholdsEntry> Thresholds { get; }
        public List<SpellPhaseEntry> Phases { get; }

        public SpellInfo(SpellBaseInfo spellBaseBaseInfo, Spell4Entry spell4Entry)
        {
            Entry                          = spell4Entry;
            BaseInfo                       = spellBaseBaseInfo;
            AoeTargetConstraints           = GameTableManager.Instance.Spell4AoeTargetConstraints.GetEntry(spell4Entry.Spell4AoeTargetConstraintsId);
            CasterConditions               = GameTableManager.Instance.Spell4Conditions.GetEntry(spell4Entry.Spell4ConditionsIdCaster);
            TargetConditions               = GameTableManager.Instance.Spell4Conditions.GetEntry(spell4Entry.Spell4ConditionsIdTarget);
            CasterCCConditions             = GameTableManager.Instance.Spell4CCConditions.GetEntry(spell4Entry.Spell4CCConditionsIdCaster);
            TargetCCConditions             = GameTableManager.Instance.Spell4CCConditions.GetEntry(spell4Entry.Spell4CCConditionsIdTarget);
            GlobalCooldown                 = GameTableManager.Instance.SpellCoolDown.GetEntry(spell4Entry.SpellCoolDownIdGlobal);
            StackGroup                     = GameTableManager.Instance.Spell4StackGroup.GetEntry(spell4Entry.Spell4StackGroupId);
            CasterCastPrerequisites        = GameTableManager.Instance.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdCasterCast);
            TargetCastPrerequisites        = GameTableManager.Instance.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdTargetCast);
            CasterPersistencePrerequisites = GameTableManager.Instance.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdCasterPersistence);
            TargetPersistencePrerequisites = GameTableManager.Instance.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdTargetPersistence);

            Telegraphs = GlobalSpellManager.Instance.GetTelegraphDamageEntries(spell4Entry.Id).ToList();
            Effects = GlobalSpellManager.Instance.GetSpell4EffectEntries(spell4Entry.Id).ToList();
            Thresholds = GlobalSpellManager.Instance.GetSpell4ThresholdEntries(spell4Entry.Id).ToList();
            Phases = GlobalSpellManager.Instance.GetSpellPhaseEntries(spell4Entry.Id).ToList();
        }
    }
}
