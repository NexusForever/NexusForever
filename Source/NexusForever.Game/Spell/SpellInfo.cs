using NexusForever.Game.Abstract.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell
{
    public class SpellInfo : ISpellInfo
    {
        public Spell4Entry Entry { get; }
        public ISpellBaseInfo BaseInfo { get; }
        public Spell4AoeTargetConstraintsEntry AoeTargetConstraints { get; }
        public Spell4ConditionsEntry CasterConditions { get; }
        public Spell4ConditionsEntry TargetConditions { get; }
        public Spell4CCConditionsEntry CasterCCConditions { get; }
        public Spell4CCConditionsEntry TargetCCConditions { get; }
        public SpellCoolDownEntry GlobalCooldown { get; }
        public Spell4StackGroupEntry StackGroup { get; }
        public Spell4GroupListEntry GroupList { get; }
        public PrerequisiteEntry CasterCastPrerequisite { get; }
        public PrerequisiteEntry TargetCastPrerequisites { get; }
        public PrerequisiteEntry CasterPersistencePrerequisites { get; }
        public PrerequisiteEntry TargetPersistencePrerequisites { get; }
        public List<PrerequisiteEntry> PrerequisiteRunners { get; } = new();

        public List<TelegraphDamageEntry> Telegraphs { get; }
        public List<Spell4EffectsEntry> Effects { get; }
        public List<Spell4ThresholdsEntry> Thresholds { get; }
        public List<SpellPhaseEntry> Phases { get; }

        public Spell4VisualGroupEntry VisualGroup { get; }
        public List<Spell4VisualEntry> Visuals { get; } = new();
        public List<SpellCoolDownEntry> Cooldowns { get; } = new();

        private Dictionary<int /* orderIndex */, ISpellInfo /* spell4Id */> thresholdCache = new();
        private (ISpellInfo, Spell4ThresholdsEntry) maxThresholdSpell;

        public SpellInfo(ISpellBaseInfo spellBaseBaseInfo, Spell4Entry spell4Entry)
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
            GroupList                      = GameTableManager.Instance.Spell4GroupList.GetEntry(spell4Entry.Spell4GroupListId);
            CasterCastPrerequisite         = GameTableManager.Instance.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdCasterCast);
            TargetCastPrerequisites        = GameTableManager.Instance.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdTargetCast);
            CasterPersistencePrerequisites = GameTableManager.Instance.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdCasterPersistence);
            TargetPersistencePrerequisites = GameTableManager.Instance.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdTargetPersistence);

            Telegraphs                     = GlobalSpellManager.Instance.GetTelegraphDamageEntries(spell4Entry.Id).ToList();
            Effects                        = GlobalSpellManager.Instance.GetSpell4EffectEntries(spell4Entry.Id).ToList();
            Thresholds                     = GlobalSpellManager.Instance.GetSpell4ThresholdEntries(spell4Entry.Id).ToList();
            Phases                         = GlobalSpellManager.Instance.GetSpellPhaseEntries(spell4Entry.Id).ToList();
            VisualGroup                    = GameTableManager.Instance.Spell4VisualGroup.GetEntry(spell4Entry.Spell4VisualGroupId);

            if (VisualGroup != null)
                foreach (uint visual in VisualGroup.Spell4VisualIdVisuals.Where(i => i != 0).ToList())
                {
                    Spell4VisualEntry visualEntry = GameTableManager.Instance.Spell4Visual.GetEntry(visual);
                    if (visualEntry != null)
                        Visuals.Add(visualEntry);
                }

            // Add all Prerequisites that allow the Caster to cast this Spell
            foreach (uint runnerId in spell4Entry.PrerequisiteIdRunners.Where(r => r != 0))
                PrerequisiteRunners.Add(GameTableManager.Instance.Prerequisite.GetEntry(runnerId));

            foreach (uint cooldownId in spell4Entry.SpellCoolDownIds)
            {
                if (cooldownId == 0)
                    continue;

                Cooldowns.Add(GameTableManager.Instance.SpellCoolDown.GetEntry(cooldownId));
            }
        }

        public void Initialise()
        {
            InitialiseThresholdCache();
        }

        private void InitialiseThresholdCache()
        {
            foreach (Spell4ThresholdsEntry thresholdsEntry in Thresholds)
            {
                Spell4Entry spell4Entry = GameTableManager.Instance.Spell4.GetEntry(thresholdsEntry.Spell4IdToCast);
                if (spell4Entry == null)
                    continue;

                ISpellBaseInfo spellBaseInfo = GlobalSpellManager.Instance.GetSpellBaseInfo(spell4Entry.Spell4BaseIdBaseSpell);
                if (spellBaseInfo == null)
                    throw new ArgumentOutOfRangeException();

                ISpellInfo spellInfo = spellBaseInfo.GetSpellInfo((byte)spell4Entry.TierIndex);
                if (spellInfo == null)
                    throw new ArgumentOutOfRangeException();

                thresholdCache.TryAdd((int)thresholdsEntry.OrderIndex, spellInfo);
            }

            if (thresholdCache.Keys.Count > 0)
                maxThresholdSpell = (thresholdCache.Last().Value, Thresholds.MaxBy(x => x.OrderIndex));
        }

        /// <summary>
        /// Return <see cref="ISpellInfo"/> for a given Threshold Index.
        /// </summary>
        public (ISpellInfo, Spell4ThresholdsEntry) GetThresholdSpellInfo(int index)
        {
            return thresholdCache.TryGetValue(index, out ISpellInfo value) ? (value, Thresholds[index]) : maxThresholdSpell;
        }
    }
}
