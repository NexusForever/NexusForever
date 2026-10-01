using NexusForever.Game.Abstract.Spell.Info;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;

namespace NexusForever.Game.Spell.Info
{
    public class SpellInfo : ISpellInfo
    {
        public Spell4Entry Entry { get; private set; }
        
        public ISpellBaseInfo BaseInfo
        {
            get => baseInfo;
            set
            {
                if (baseInfo != null)
                    throw new InvalidOperationException("BaseInfo already set.");

                baseInfo = value;
            }
        }
        private ISpellBaseInfo baseInfo;

        public Spell4AoeTargetConstraintsEntry AoeTargetConstraints { get; private set; }
        public Spell4ConditionsEntry CasterConditions { get; private set; }
        public Spell4ConditionsEntry TargetConditions { get; private set; }
        public Spell4CCConditionsEntry CasterCCConditions { get; private set; }
        public Spell4CCConditionsEntry TargetCCConditions { get; private set; }
        public SpellCoolDownEntry GlobalCooldown { get; private set; }
        public Spell4StackGroupEntry StackGroup { get; private set; }
        public PrerequisiteEntry CasterCastPrerequisite { get; private set; }
        public PrerequisiteEntry TargetCastPrerequisite { get; private set; }
        public PrerequisiteEntry CasterPersistencePrerequisite { get; private set; }
        public PrerequisiteEntry TargetPersistencePrerequisite { get; private set; }
        public List<PrerequisiteEntry> PrerequisiteRunners { get; private set; } = [];

        public List<TelegraphDamageEntry> Telegraphs { get; private set; }
        public List<Spell4EffectsEntry> Effects { get; private set; }
        public List<Spell4ThresholdsEntry> Thresholds { get; private set; }
        public List<SpellPhaseEntry> Phases { get; private set; }

        public Spell4VisualGroupEntry VisualGroup { get; private set; }
        public List<Spell4VisualEntry> Visuals { get; private set; } = [];
        public List<SpellCoolDownEntry> Cooldowns { get; private set; } = [];

        public HashSet<uint> SpellGroups { get; private set; } = [];

        private readonly Dictionary<int /* orderIndex */, ISpellInfo /* spell4Id */> thresholdCache = [];
        private (ISpellInfo, Spell4ThresholdsEntry) maxThresholdSpell;

        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;
        private readonly ISpellInfoManager spellInfoManager;

        public SpellInfo(
            IGameTableManager gameTableManager,
            ISpellInfoManager spellInfoManager)
        {
            this.gameTableManager = gameTableManager;
            this.spellInfoManager = spellInfoManager;
        }

        #endregion

        /// <summary>
        /// Initialise the <see cref="ISpellInfo"/> with the supplied <see cref="Spell4Entry"/> and <see cref="ISpellInfoCache"/>.
        /// </summary>
        public void Initialise(Spell4Entry spell4Entry, ISpellInfoCache spellInfoCache)
        {
            if (Entry != null)
                throw new InvalidOperationException("SpellInfo already initialised.");

            Entry                          = spell4Entry;
            AoeTargetConstraints           = gameTableManager.Spell4AoeTargetConstraints.GetEntry(spell4Entry.Spell4AoeTargetConstraintsId);
            CasterConditions               = gameTableManager.Spell4Conditions.GetEntry(spell4Entry.Spell4ConditionsIdCaster);
            TargetConditions               = gameTableManager.Spell4Conditions.GetEntry(spell4Entry.Spell4ConditionsIdTarget);
            CasterCCConditions             = gameTableManager.Spell4CCConditions.GetEntry(spell4Entry.Spell4CCConditionsIdCaster);
            TargetCCConditions             = gameTableManager.Spell4CCConditions.GetEntry(spell4Entry.Spell4CCConditionsIdTarget);
            GlobalCooldown                 = gameTableManager.SpellCoolDown.GetEntry(spell4Entry.SpellCoolDownIdGlobal);
            StackGroup                     = gameTableManager.Spell4StackGroup.GetEntry(spell4Entry.Spell4StackGroupId);
            CasterCastPrerequisite         = gameTableManager.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdCasterCast);
            TargetCastPrerequisite         = gameTableManager.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdTargetCast);
            CasterPersistencePrerequisite  = gameTableManager.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdCasterPersistence);
            TargetPersistencePrerequisite  = gameTableManager.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdTargetPersistence);

            Telegraphs                     = spellInfoCache.GetTelegraphDamageEntries(spell4Entry.Id).ToList();
            Effects                        = spellInfoCache.GetSpell4EffectEntries(spell4Entry.Id).ToList();
            Thresholds                     = spellInfoCache.GetSpell4ThresholdEntries(spell4Entry.Id).ToList();
            Phases                         = spellInfoCache.GetSpellPhaseEntries(spell4Entry.Id).ToList();
            VisualGroup                    = gameTableManager.Spell4VisualGroup.GetEntry(spell4Entry.Spell4VisualGroupId);

            if (VisualGroup != null)
            {
                foreach (uint visual in VisualGroup.Spell4VisualIdVisuals.Where(i => i != 0).ToList())
                {
                    Spell4VisualEntry visualEntry = gameTableManager.Spell4Visual.GetEntry(visual);
                    if (visualEntry != null)
                        Visuals.Add(visualEntry);
                }
            }
                
            // Add all Prerequisites that allow the Caster to cast this Spell
            foreach (uint runnerId in spell4Entry.PrerequisiteIdRunners.Where(r => r != 0))
                PrerequisiteRunners.Add(gameTableManager.Prerequisite.GetEntry(runnerId));

            foreach (uint cooldownId in spell4Entry.SpellCoolDownIds)
            {
                if (cooldownId == 0)
                    continue;

                Cooldowns.Add(gameTableManager.SpellCoolDown.GetEntry(cooldownId));
            }

            if (spell4Entry.Spell4GroupListId != 0)
            {
                Spell4GroupListEntry groupList = gameTableManager.Spell4GroupList.GetEntry(spell4Entry.Spell4GroupListId);
                if (groupList != null)
                    SpellGroups.UnionWith(groupList.SpellGroupIds);
            }
        }

        /// <summary>
        /// Perform any late initialisation tasks that need to complete after all <see cref="ISpellInfo"/> have been initialised.
        /// </summary>
        public void InitialiseLate()
        {
            InitialiseThresholdCache();
        }

        private void InitialiseThresholdCache()
        {
            foreach (Spell4ThresholdsEntry thresholdsEntry in Thresholds)
            {
                ISpellInfo spellInfo = spellInfoManager.GetSpellInfo(thresholdsEntry.Spell4IdToCast);
                if (spellInfo == null)
                    continue;
                
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
