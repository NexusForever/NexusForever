using NexusForever.Game.Static.Quest;

namespace NexusForever.Game.Abstract.Quest
{
    /// <summary>
    /// Arms a player's communicator calls from quest, zone and level events and delivers them once their conditions hold.
    /// </summary>
    public interface ICommunicatorTracker
    {
        ICommunicatorPlayerView View { get; }

        /// <summary>
        /// A quest's state was set. <paramref name="created"/> is the first set on a new quest record, whose old state
        /// is only the constructor's default.
        /// </summary>
        void OnQuestState(ushort questId, QuestState oldState, QuestState newState, bool created = false);

        /// <summary>
        /// A quest objective was completed.
        /// </summary>
        void OnObjectiveComplete(ushort questId, byte objectiveIndex);
    }
}
