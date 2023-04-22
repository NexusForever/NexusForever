using NexusForever.GameTable.Model;
using System.Collections.Immutable;

namespace NexusForever.Game.Abstract.Quest
{
    public interface IQuestInfo
    {
        Quest2Entry Entry { get; }
        Quest2DifficultyEntry DifficultyEntry { get; }

        ImmutableList<Quest2Entry> PrerequisiteQuests { get; }
        ImmutableList<IQuestObjectiveInfo> Objectives { get; }
        ImmutableDictionary<uint, Quest2RewardEntry> Rewards { get; }

        bool IsQuestMentioned { get; }

        bool IsAutoComplete();
        bool IsCommunicatorReceived();
        bool CanBeCalledBack();
        bool CannotAbandon();
        bool CannotAbandonWhenAchieved();
        bool IsContract();

        /// <summary>
        /// Return experience rewarded on completion.
        /// </summary>
        uint GetRewardExperience();

        /// <summary>
        /// Return money rewarded on completion.
        /// </summary>
        uint GetRewardMoney();
    }
}