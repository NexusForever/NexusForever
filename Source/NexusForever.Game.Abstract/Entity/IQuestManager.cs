using NexusForever.Database.Character;
using NexusForever.Game.Abstract.Quest;
using NexusForever.Game.Static.Quest;
using NexusForever.Shared;

namespace NexusForever.Game.Abstract.Entity
{
    public interface IQuestManager : IDatabaseCharacter, IUpdate
    {
        void SendInitialPackets();

        /// <summary>
        /// Return <see cref="QuestState"/> for supplied quest.
        /// </summary>
        QuestState? GetQuestState(ushort questId);

        /// <summary>
        /// Mention a quest from supplied quest id, skipping any prerequisites checks.
        /// </summary>
        void QuestMention(ushort questId);

        /// <summary>
        /// Mention a quest from supplied <see cref="IQuestInfo"/>, skipping any prerequisites checks.
        /// </summary>
        void QuestMention(IQuestInfo info);

        /// <summary>
        /// Add a quest from supplied id, optionally supplying <see cref="IItem"/> which was used to start the quest.
        /// </summary>
        void QuestAdd(ushort questId, IItem item);

        /// <summary>
        /// Add a quest from supplied <see cref="IQuestInfo"/>, skipping any prerequisites checks.
        /// </summary>
        void QuestAdd(IQuestInfo info);

        /// <summary>
        /// Retry an inactive quest id that was previously failed.
        /// </summary>
        void QuestRetry(ushort questId);

        /// <summary>
        /// Abandon an active quest.
        /// </summary>
        void QuestAbandon(ushort questId);

        /// <summary>
        /// Complete all <see cref="IQuestObjective"/>'s for supplied active quest id.
        /// </summary>
        void QuestAchieve(ushort questId);

        /// <summary>
        /// Complete single <see cref="IQuestObjective"/> for supplied active quest id.
        /// </summary>
        void QuestAchieveObjective(ushort questId, byte index);

        /// <summary>
        /// Complete an achieved quest supplying an optional reward and whether the quest was completed from the communicator.
        /// </summary>
        void QuestComplete(ushort questId, ushort reward, bool communicator);

        /// <summary>
        /// Ignore or acknowledge an inactive quest.
        /// </summary>
        void QuestIgnore(ushort questId, bool ignored);

        /// <summary>
        /// Track or hide an active quest.
        /// </summary>
        void QuestTrack(ushort questId, bool tracked);

        /// <summary>
        /// Share supplied quest with another <see cref="IPlayer"/>.
        /// </summary>
        void QuestShare(ushort questId);

        /// <summary>
        /// Accept or deny a shared quest from another <see cref="IPlayer"/>.
        /// </summary>
        void QuestShareResult(ushort questId, bool result);

        /// <summary>
        /// Update any active quest <see cref="IQuestObjective"/>'s with supplied <see cref="QuestObjectiveType"/> and data with progress.
        /// </summary>
        void ObjectiveUpdate(QuestObjectiveType type, uint data, uint progress);

        // <summary>
        /// Update any active quest <see cref="IQuestObjective"/>'s with supplied ID with progress.
        /// </summary>
        void ObjectiveUpdate(uint id, uint progress);

        /// <summary>
        /// Returns a collection of all active quests.
        /// </summary>
        IEnumerable<IQuest> GetActiveQuests();
    }
}