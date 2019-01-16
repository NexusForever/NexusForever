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

        public SpellInfo(SpellBaseInfo spellBaseBaseInfo, Spell4Entry spell4Entry)
        {
            Entry                          = spell4Entry;
            BaseInfo                       = spellBaseBaseInfo;
            AoeTargetConstraints           = GameTableManager.Spell4AoeTargetConstraints.GetEntry(spell4Entry.Spell4AoeTargetConstraintsId);
            CasterConditions               = GameTableManager.Spell4Conditions.GetEntry(spell4Entry.Spell4ConditionsIdCaster);
            TargetConditions               = GameTableManager.Spell4Conditions.GetEntry(spell4Entry.Spell4ConditionsIdTarget);
            CasterCCConditions             = GameTableManager.Spell4CCConditions.GetEntry(spell4Entry.Spell4CCConditionsIdCaster);
            TargetCCConditions             = GameTableManager.Spell4CCConditions.GetEntry(spell4Entry.Spell4CCConditionsIdTarget);
            GlobalCooldown                 = GameTableManager.SpellCoolDown.GetEntry(spell4Entry.SpellCoolDownIdGlobal);
            StackGroup                     = GameTableManager.Spell4StackGroup.GetEntry(spell4Entry.Spell4StackGroupId);
            CasterCastPrerequisites        = GameTableManager.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdCasterCast);
            TargetCastPrerequisites        = GameTableManager.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdTargetCast);
            CasterPersistencePrerequisites = GameTableManager.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdCasterPersistence);
            TargetPersistencePrerequisites = GameTableManager.Prerequisite.GetEntry(spell4Entry.PrerequisiteIdTargetPersistence);

            Telegraphs = GameTableManager.Spell4Telegraph.Entries
                .Where(e => e.Spell4Id == Entry.Id)
                .Select(e => GameTableManager.TelegraphDamage.GetEntry(e.TelegraphDamageId))
                .ToList();

            Effects = GameTableManager.Spell4Effects.Entries
                .Where(e => e.SpellId == spell4Entry.Id)
                .OrderBy(e => e.OrderIndex)
                .ToList();
        }
    }
}
