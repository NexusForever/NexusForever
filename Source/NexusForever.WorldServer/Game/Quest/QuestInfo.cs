using System;
using System.Collections.Generic;
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
        public ImmutableList<QuestObjectiveEntry> Objectives { get; }
        public ImmutableDictionary<uint, Quest2RewardEntry> Rewards { get; }

        /// <summary>
        /// Create a new <see cref="QuestInfo"/> using supplied <see cref="Quest2Entry"/>.
        /// </summary>
        public QuestInfo(Quest2Entry entry)
        {
            Entry           = entry;
            DifficultyEntry = GameTableManager.Instance.Quest2Difficulty.GetEntry(Entry.Quest2DifficultyId);

            ImmutableList<QuestObjectiveEntry>.Builder objectiveBuilder = ImmutableList.CreateBuilder<QuestObjectiveEntry>();
            foreach (uint objectiveId in entry.Objectives.Where(o => o != 0u))
                objectiveBuilder.Add(GameTableManager.Instance.QuestObjective.GetEntry(objectiveId));

            Objectives = objectiveBuilder.ToImmutable();

            ImmutableDictionary<uint, Quest2RewardEntry>.Builder rewardBuilder = ImmutableDictionary.CreateBuilder<uint, Quest2RewardEntry>();
            foreach (Quest2RewardEntry rewardEntry in GameTableManager.Instance.Quest2Reward.Entries
                .Where(e => e.Quest2Id == entry.Id))
                rewardBuilder.Add(rewardEntry.Id, rewardEntry);

            Rewards = rewardBuilder.ToImmutable();
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
            return (Entry.Flags & 0x08) != 0;
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

        /// <summary>
        /// Return reputation rewarded on completion.
        /// </summary>
        public Dictionary<uint, float> GetRewardReputation()
        {
            Dictionary<uint, float> reputationRewards = new Dictionary<uint, float>();

            for (int i = 0; i < Entry.Faction2IdRewardReputations.Length - 1; i++)
            {
                uint faction2Id = Entry.Faction2IdRewardReputations[i];

                if (faction2Id == 0u)
                    continue;

                if (Entry.RewardReputationOverrides[i] > 0f)
                {
                    reputationRewards.Add(faction2Id, Entry.RewardReputationOverrides[i]);
                    continue;
                }

                XpPerLevelEntry entry = GameTableManager.Instance.XpPerLevel.GetEntry(Entry.ConLevel);
                reputationRewards.Add(faction2Id, DifficultyEntry.RepRewardMultiplier * entry.BaseRepRewardPerLevel);
            }

            return reputationRewards;
        }
    }
}
