using System;
using System.Collections.Immutable;
using System.Linq;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.WorldServer.GameTable.Quest2.Static;

namespace NexusForever.WorldServer.Game.Quest
{
    public class QuestInfo
    {
        public Quest2Entry Entry { get; }
        public Quest2DifficultyEntry DifficultyEntry { get; }

        public ImmutableList<Quest2Entry> PrerequisiteQuests { get; private set; }
        public ImmutableList<QuestObjectiveInfo> Objectives { get; private set; }
        public ImmutableDictionary<uint, Quest2RewardEntry> Rewards { get; private set; }

        public bool IsQuestMentioned => GlobalQuestManager.Instance.GetQuestCommunicatorMessages((ushort)Entry.Id).ToList().Count > 0u;

        /// <summary>
        /// Create a new <see cref="QuestInfo"/> using supplied <see cref="Quest2Entry"/>.
        /// </summary>
        public QuestInfo(Quest2Entry entry)
        {
            Entry           = entry;
            DifficultyEntry = GameTableManager.Instance.Quest2Difficulty.GetEntry(Entry.Quest2DifficultyId);

            InitialisePrerequisiteQuests();
            InitialiseObjectives();
            InitialiseRewards();
        }

        private void InitialisePrerequisiteQuests()
        {
            ImmutableList<Quest2Entry>.Builder builder = ImmutableList.CreateBuilder<Quest2Entry>();
            foreach (uint questId in Entry.PrerequisiteQuests.Where(q => q != 0u))
                builder.Add(GameTableManager.Instance.Quest2.GetEntry(questId));

            PrerequisiteQuests = builder.ToImmutable();
        }

        private void InitialiseObjectives()
        {
            ImmutableList<QuestObjectiveInfo>.Builder builder = ImmutableList.CreateBuilder<QuestObjectiveInfo>();
            foreach (uint objectiveId in Entry.Objectives.Where(o => o != 0u))
                builder.Add(new QuestObjectiveInfo(GameTableManager.Instance.QuestObjective.GetEntry(objectiveId)));

            Objectives = builder.ToImmutable();
        }

        private void InitialiseRewards()
        {
            ImmutableDictionary<uint, Quest2RewardEntry>.Builder builder = ImmutableDictionary.CreateBuilder<uint, Quest2RewardEntry>();
            foreach (Quest2RewardEntry rewardEntry in GameTableManager.Instance.Quest2Reward.Entries
                .Where(e => e.Quest2Id == Entry.Id))
                builder.Add(rewardEntry.Id, rewardEntry);

            Rewards = builder.ToImmutable();
        }

        public bool IsAutoComplete()
        {
            return ((QuestFlags)Entry.Flags & QuestFlags.AutoComplete) != 0;
        }

        /// <summary>
        /// Returns if the quest can be accepted and completed via the communicator.
        /// </summary>
        public bool IsCommunicatorReceived()
        {
            return (Entry.Flags & 0x08) != 0 ;
        }

        public bool CanBeCalledBack()
        {
            return (Entry.Flags & 0x04) != 0;
        }

        public bool CannotAbandon()
        {
            return (Entry.Flags & 0x4000) != 0;
        }

        public bool CannotAbandonWhenAchieved()
        {
            return (Entry.Flags & 0x8000) != 0;
        }

        public bool IsContract()
        {
            return (Entry.Flags & 0x80000) != 0;
        }

        /// <summary>
        /// Return experience rewarded on completion.
        /// </summary>
        public uint GetRewardExperience()
        {
            if (Entry.RewardXpOverride != 0u)
                return Entry.RewardXpOverride;

            XpPerLevelEntry entry = GameTableManager.Instance.XpPerLevel.GetEntry(Entry.ConLevel);
            return (uint)(DifficultyEntry.XpMultiplier * entry.BaseQuestXpPerLevel);
        }

        /// <summary>
        /// Return money rewarded on completion.
        /// </summary>
        public uint GetRewardMoney()
        {
            if (Entry.RewardCashOverride != 0u)
                return Entry.RewardCashOverride;

            GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(530);
            return (uint)(MathF.Pow(Entry.ConLevel, entry.Datafloat0) * DifficultyEntry.CashRewardMultiplier);
        }
    }
}
