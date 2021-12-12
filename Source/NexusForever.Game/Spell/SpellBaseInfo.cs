using NexusForever.Game.Abstract.Spell;
using NexusForever.Game.Static.Spell;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using System.Collections;

namespace NexusForever.Game.Spell
{
    public class SpellBaseInfo : ISpellBaseInfo
    {
        public Spell4BaseEntry Entry { get; }
        public Spell4HitResultsEntry HitResult { get; }
        public Spell4TargetMechanicsEntry TargetMechanics { get; }
        public Spell4TargetAngleEntry TargetAngle { get; }
        public Spell4PrerequisitesEntry Prerequisites { get; }
        public Spell4ValidTargetsEntry ValidTargets { get; }
        public TargetGroupEntry CastGroup { get; }
        public Creature2Entry PositionalAoe { get; }
        public TargetGroupEntry AoeGroup { get; }
        public Spell4BaseEntry PrerequisiteSpell { get; }
        public Spell4SpellTypesEntry SpellType { get; }
        public SpellClass SpellClass { get; }
        public bool HasIcon { get; }
        public bool IsDebuff { get; }
        public bool IsBuff { get; }
        public bool IsDispellable { get; }

        private readonly ISpellInfo[] spellInfoStore;

        public SpellBaseInfo(Spell4BaseEntry spell4BaseEntry)
        {
            Entry             = spell4BaseEntry;
            HitResult         = GameTableManager.Instance.Spell4HitResults.GetEntry(Entry.Spell4HitResultId);
            TargetMechanics   = GameTableManager.Instance.Spell4TargetMechanics.GetEntry(Entry.Spell4TargetMechanicId);
            TargetAngle       = GameTableManager.Instance.Spell4TargetAngle.GetEntry(Entry.Spell4TargetAngleId);
            Prerequisites     = GameTableManager.Instance.Spell4Prerequisites.GetEntry(Entry.Spell4PrerequisiteId);
            ValidTargets      = GameTableManager.Instance.Spell4ValidTargets.GetEntry(Entry.Spell4ValidTargetId);
            CastGroup         = GameTableManager.Instance.TargetGroup.GetEntry(Entry.TargetGroupIdCastGroup);
            PositionalAoe     = GameTableManager.Instance.Creature2.GetEntry(Entry.Creature2IdPositionalAoe);
            AoeGroup          = GameTableManager.Instance.TargetGroup.GetEntry(Entry.TargetGroupIdAoeGroup);
            PrerequisiteSpell = GameTableManager.Instance.Spell4Base.GetEntry(Entry.Spell4BaseIdPrerequisiteSpell);
            SpellType         = GameTableManager.Instance.Spell4SpellTypes.GetEntry(Entry.Spell4SpellTypesIdSpellType);

            SpellClass        = (SpellClass)Entry.SpellClass;
            HasIcon           = SpellClass == SpellClass.BuffNonDispelRightClickOk || (SpellClass >= SpellClass.BuffDispellable && SpellClass <= SpellClass.DebuffNonDispellable);
            IsDebuff          = SpellClass == SpellClass.DebuffDispellable || SpellClass == SpellClass.DebuffNonDispellable;
            IsBuff            = SpellClass == SpellClass.BuffDispellable || SpellClass == SpellClass.BuffNonDispellable || SpellClass == SpellClass.BuffNonDispelRightClickOk;
            IsDispellable     = SpellClass == SpellClass.BuffDispellable || SpellClass == SpellClass.DebuffDispellable;

            List<Spell4Entry> spellEntries = GlobalSpellManager.Instance.GetSpell4Entries(spell4BaseEntry.Id).ToList();
            if (spellEntries.Count < 1)
                return;

            // spell don't always have sequential tiers, create from highest tier not total
            if (spellInfoStore == null)
                spellInfoStore = new SpellInfo[spellEntries[0].TierIndex];

            foreach (Spell4Entry spell4Entry in spellEntries)
            {
                var spellInfo = new SpellInfo(this, spell4Entry);
                spellInfoStore[spell4Entry.TierIndex - 1] = spellInfo;
            }
        }

        /// <summary>
        /// Initialise this <see cref="ISpellBaseInfo"/>.
        /// </summary>
        public void Intitialise()
        {
            if (spellInfoStore == null)
                return;

            foreach (ISpellInfo spell in spellInfoStore)
            {
                if (spell == null)
                    continue;

                spell.Initialise();
            }
        }

        /// <summary>
        /// Return <see cref="ISpellInfo"/> for the supplied spell tier.
        /// </summary>
        public ISpellInfo GetSpellInfo(byte tier)
        {
            if (tier < 1)
                tier = 1;
            return spellInfoStore[tier - 1];
        }

        public IEnumerator<ISpellInfo> GetEnumerator()
        {
            return spellInfoStore
                .Select(s => s)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
