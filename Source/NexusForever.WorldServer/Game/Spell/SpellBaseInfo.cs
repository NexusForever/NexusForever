using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;

namespace NexusForever.WorldServer.Game.Spell
{
    public class SpellBaseInfo
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

        private readonly SpellInfo[] spellInfoStore;

        public SpellBaseInfo(Spell4BaseEntry spell4BaseEntry)
        {
            Entry             = spell4BaseEntry;
            HitResult         = GameTableManager.Spell4HitResults.GetEntry(Entry.Spell4HitResultId);
            TargetMechanics   = GameTableManager.Spell4TargetMechanics.GetEntry(Entry.Spell4TargetMechanicId);
            TargetAngle       = GameTableManager.Spell4TargetAngle.GetEntry(Entry.Spell4TargetAngleId);
            Prerequisites     = GameTableManager.Spell4Prerequisites.GetEntry(Entry.Spell4PrerequisiteId);
            ValidTargets      = GameTableManager.Spell4ValidTargets.GetEntry(Entry.Spell4ValidTargetId);
            CastGroup         = GameTableManager.TargetGroup.GetEntry(Entry.TargetGroupIdCastGroup);
            PositionalAoe     = GameTableManager.Creature2.GetEntry(Entry.Creature2IdPositionalAoe);
            AoeGroup          = GameTableManager.TargetGroup.GetEntry(Entry.TargetGroupIdAoeGroup);
            PrerequisiteSpell = GameTableManager.Spell4Base.GetEntry(Entry.Spell4BaseIdPrerequisiteSpell);
            SpellType         = GameTableManager.Spell4SpellTypes.GetEntry(Entry.Spell4SpellTypesIdSpellType);

            foreach (Spell4Entry spell4Entry in GameTableManager.Spell4.Entries
                .Where(e => e.Spell4BaseIdBaseSpell == Entry.Id)
                .OrderByDescending(e => e.TierIndex))
            {
                // spell don't always have sequential tiers, create from highest tier not total
                if (spellInfoStore == null)
                    spellInfoStore = new SpellInfo[spell4Entry.TierIndex];

                var spellInfo = new SpellInfo(this, spell4Entry);
                spellInfoStore[spell4Entry.TierIndex - 1] = spellInfo;
            }
        }

        /// <summary>
        /// Return <see cref="SpellInfo"/> for the supplied spell tier.
        /// </summary>
        public SpellInfo GetSpellInfo(byte tier)
        {
            if (tier < 1)
                tier = 1;
            return spellInfoStore[tier - 1];
        }
    }
}
